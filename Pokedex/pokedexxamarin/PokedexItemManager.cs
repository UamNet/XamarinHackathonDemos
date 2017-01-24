/*
 * To add Offline Sync Support:
 *  1) Add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore (and dependencies) to all client projects
 *  2) Uncomment the #define OFFLINE_SYNC_ENABLED
 *
 * For more information, see: http://go.microsoft.com/fwlink/?LinkId=620342
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace pokedexxamarin
{
    public partial class PokedexItemManager
    {
        static PokedexItemManager defaultInstance = new PokedexItemManager();
        MobileServiceClient client;

        CloudBlobContainer container;
        CloudBlobClient blobClient;
        CloudStorageAccount storageAccount;

        IMobileServiceSyncTable<Pokemon> todoTable;

        const string offlineDbPath = @"localstorePokedex.db";

        private PokedexItemManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);

            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<Pokemon>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.todoTable = client.GetSyncTable<Pokemon>();

            //Blob storage

            storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=pokedex;AccountKey=7p57hehIeLmgfqlVocGCIrnuvugW26biMosYkLkz3QJwcK8EiCwzQr/LZZERA0y/XsrwEffsQVEOssvD4if2eg==");

            // Create the blob client.
            blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            container = blobClient.GetContainerReference("photos");

            // Create the container if it doesn't already exist.
            container.CreateIfNotExistsAsync();

        }

        public static PokedexItemManager DefaultManager
        {
            get
            {
                return defaultInstance;
            }
            private set
            {
                defaultInstance = value;
            }
        }

        public Pokemon pendingItem { get; set; }

        public MobileServiceClient CurrentClient
        {
            get { return client; }
        }

        public bool IsOfflineEnabled
        {
            get { return todoTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Pokemon>; }
        }

        public String getContainerToken()
        {
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read;
            return container.GetSharedAccessSignature(sasConstraints);
        }

        public async Task<ObservableCollection<Pokemon>> GetPokedexItemsAsync(bool syncItems = false)
        {
            try
            {
                if (syncItems)
                {
                    await this.SyncAsync();
                }

                SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
                sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24);
                sasConstraints.Permissions = SharedAccessBlobPermissions.Read;
                string sasContainerToken = container.GetSharedAccessSignature(sasConstraints);

                IEnumerable<Pokemon> items = await todoTable
                    //.Where(Pokemon => !Pokemon.Done)
                    .Select(oldItem => new Pokemon()
                    {
                        Name = oldItem.Name,
                        Age = oldItem.Age,
                        Gender = oldItem.Gender,
                        Id = oldItem.Id,
                        Version = oldItem.Id,
                        Description= oldItem.Description,
                        Emotion=oldItem.Emotion,
                        Img = container.GetBlockBlobReference(oldItem.Img).Uri + sasContainerToken
                    })
                    .ToEnumerableAsync();

                return new ObservableCollection<Pokemon>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }

        public async Task SaveTaskAsync(Pokemon item)
        {
            if (item.Id == null)
            {
                await todoTable.InsertAsync(item);
            }
            else
            {
                await todoTable.UpdateAsync(item);
            }
        }

        public async Task<String> UploadPicture(String id, System.IO.Stream stream)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(id);

            await blockBlob.UploadFromStreamAsync(stream);

            return blockBlob.Uri + this.getContainerToken();
        }

        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.todoTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allPokemons",
                    this.todoTable.CreateQuery());
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                {
                    syncErrors = exc.PushResult.Errors;
                }
            }

            // Simple error/conflict handling. A real application would handle the various errors like network conditions,
            // server conflicts and others via the IMobileServiceSyncHandler.
            if (syncErrors != null)
            {
                foreach (var error in syncErrors)
                {
                    if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                    {
                        //Update failed, reverting to server's copy.
                        await error.CancelAndUpdateItemAsync(error.Result);
                    }
                    else
                    {
                        // Discard local change.
                        await error.CancelAndDiscardItemAsync();
                    }

                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
                }
            }
        }
    }
}
