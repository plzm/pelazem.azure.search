using System;
using System.Threading.Tasks;
using pelazem.azure.search;

namespace Tester
{
	class Program
	{
		static void Main(string[] args)
		{
			DoIt().Wait();
		}

		public static async Task DoIt()
		{
			string searchServiceName = "";
			string searchServiceQueryApiKey = "";
			string indexName = "";

			string filterText = "";

			ApiClient apiClient = new ApiClient(searchServiceName, searchServiceQueryApiKey, indexName);

			var results = await apiClient.SearchAsync(string.Empty, filterText, 10);
		}
	}
}
