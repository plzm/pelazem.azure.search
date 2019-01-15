using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace pelazem.azure.search
{
	public class ApiClient
	{
		#region Constants

		private const int MAX_CAP_RESULT_COUNT = 100;

		#endregion

		#region Variables

		private static SearchIndexClient _searchIndexClient = null;

		#endregion

		#region Properties

		public string SearchServiceName { get; private set; }
		public string SearchServiceQueryApiKey { get; private set; }
		public string IndexName { get; private set; }

		public SearchIndexClient SearchIndexClient
		{
			get
			{
				if (_searchIndexClient == null)
					_searchIndexClient = new SearchIndexClient(this.SearchServiceName, this.IndexName, new SearchCredentials(this.SearchServiceQueryApiKey));

				return _searchIndexClient;
			}
		}

		#endregion

		#region Constructors

		private ApiClient() { }

		public ApiClient(string searchServiceName, string searchServiceQueryApiKey, string indexName)
		{
			if (string.IsNullOrWhiteSpace(searchServiceName))
				throw new ArgumentException(nameof(searchServiceName));

			if (string.IsNullOrWhiteSpace(searchServiceQueryApiKey))
				throw new ArgumentException(nameof(searchServiceQueryApiKey));

			if (string.IsNullOrWhiteSpace(indexName))
				throw new ArgumentException(nameof(indexName));

			this.SearchServiceName = searchServiceName;
			this.SearchServiceQueryApiKey = searchServiceQueryApiKey;
			this.IndexName = indexName;
		}

		#endregion

		public async Task<List<SearchResult>> SearchAsync(string searchText, string filterText, int? topNumberOfResults = null)
		{
			int maxNumResults;

			if (topNumberOfResults != null && topNumberOfResults.Value > 0 && topNumberOfResults.Value <= MAX_CAP_RESULT_COUNT)
				maxNumResults = topNumberOfResults.Value;
			else
				maxNumResults = MAX_CAP_RESULT_COUNT;

			List<SearchResult> result = new List<SearchResult>();

			if (string.IsNullOrWhiteSpace(searchText))
				searchText = "*";

			SearchParameters searchParams = new SearchParameters();

			if (!string.IsNullOrWhiteSpace(filterText))
				searchParams.Filter = filterText;

			searchParams.Top = maxNumResults;

			DocumentSearchResult documentSearchResult = await this.SearchIndexClient.Documents.SearchAsync(searchText, searchParams);

			result.AddRange(documentSearchResult.Results);

			while (result.Count < maxNumResults && documentSearchResult.ContinuationToken != null)
			{
				documentSearchResult = await this.SearchIndexClient.Documents.ContinueSearchAsync(documentSearchResult.ContinuationToken);

				result.AddRange(documentSearchResult.Results);
			}

			return result;
		}
	}
}
