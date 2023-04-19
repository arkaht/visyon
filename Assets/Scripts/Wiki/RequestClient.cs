using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Visyon.Wiki
{
	public class RequestClient
	{
		public string APILink
		{
			get => httpClient.BaseAddress.AbsoluteUri;
			set => httpClient.BaseAddress = new Uri( value );
		}

		private readonly HttpClient httpClient;

		public RequestClient( string api_link )
		{
			httpClient = new(
				new HttpClientHandler()
				{
					AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
				}
			);

			APILink = api_link;
		}

		public async Task<string> Get( RequestArguments args )
		{
			var response = await httpClient.GetAsync( args.Stringify() );

			string content = response.Content.ReadAsStringAsync().Result;
			return content;
		}
	}
}
