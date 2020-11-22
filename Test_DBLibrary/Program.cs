using CosmoDBLibrary;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewModel;

namespace Test_DBLibrary
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Beginning operations...\n");
                CosmoDBDriver cosmoDBDriver = new CosmoDBDriver();
                Event eve = new Event
                {
                    Id = "ven.1",
                    Name = "ven",
                    Description = "event client"
                };
                Console.WriteLine("AddItems To Container Async operations...\n");
                await cosmoDBDriver.AddItemsToContainerAsync(eve);
                Console.WriteLine("Beginning operations Update Item ...\n");
                await cosmoDBDriver.UpdateItemAsync("Diyan", "Diyan.1");
             
                Console.WriteLine("Beginning operations Read Items  ...\n");
                List<Event> events = await cosmoDBDriver.ReadItemsAsync("ven");
                foreach (Event eves in events)
                {
                    Console.WriteLine("\tRead {0}\n", eves);
                }
                Console.WriteLine("Beginning operations Read Items  ...\n");
                List<Event> events1 = await cosmoDBDriver.ReadItemsAsync("Diyan");
                foreach (Event eves in events1)
                {
                    Console.WriteLine("\tRead {0}\n", eves);
                }
                 Console.WriteLine("Beginning operations Delete Item ...\n");
                 await cosmoDBDriver.DeleteItemAsync("Diyan", "Diyan.1");
                //Console.WriteLine("Beginning operations DeleteDatabase And Cleanup ...\n");
                //await cosmoDBDriver.DeleteDatabaseAndCleanupAsync();
            }

            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
