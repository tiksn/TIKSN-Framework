using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry.Pushalot
{
	public class PushalotClient
	{
		private const string API_SEND_MESSAGE_URL = "https://pushalot.com/api/sendmessage";
		private const string JSON_FIELD_NAME_AUTHORIZATION_TOKEN = "AuthorizationToken";
		private const string JSON_FIELD_NAME_BODY = "Body";
		private const string JSON_FIELD_NAME_IMAGE = "Image";
		private const string JSON_FIELD_NAME_IS_IMPORTANT = "IsImportant";
		private const string JSON_FIELD_NAME_IS_SILENT = "IsSilent";
		private const string JSON_FIELD_NAME_LINK = "Link";
		private const string JSON_FIELD_NAME_LINK_TITLE = "LinkTitle";
		private const string JSON_FIELD_NAME_SOURCE = "Source";
		private const string JSON_FIELD_NAME_TIME_TO_LIVE = "TimeToLive";
		private const string JSON_FIELD_NAME_TITLE = "Title";
		private const string REQUEST_CONTENT_TYPE = "application/json";

		private HashSet<PushalotAuthorizationToken> generalSubscribers;

		public PushalotClient()
		{
			generalSubscribers = new HashSet<PushalotAuthorizationToken>();
		}

		public async Task SendMessage(PushalotMessage message)
		{
			foreach (var generalSubscriber in generalSubscribers)
			{
				await SendMessage(message, generalSubscriber);
			}
		}

		public bool Subscribe(PushalotAuthorizationToken authorizationToken)
		{
			return generalSubscribers.Add(authorizationToken);
		}

		public bool Unsubscribe(PushalotAuthorizationToken authorizationToken)
		{
			return generalSubscribers.Remove(authorizationToken);
		}

		protected static async Task SendMessage(PushalotMessage message, PushalotAuthorizationToken authorizationToken)
		{
			string jsonText = GetJsonText(message, authorizationToken);

			Debug.WriteLine("JSON: {0}", jsonText);

			using (var client = new HttpClient())
			{
				using (var content = new StringContent(jsonText, Encoding.UTF8, REQUEST_CONTENT_TYPE))
				{
					using (var response = await client.PostAsync(API_SEND_MESSAGE_URL, content))
					{
						switch (response.StatusCode)
						{
							case HttpStatusCode.OK:
								break;

							case HttpStatusCode.BadRequest:
								throw new Exception("Input data validation failed. Check result information Description field for detailed information. Report about this issue by visiting https://pushalot.codeplex.com/workitem/list/basic.");
							case HttpStatusCode.MethodNotAllowed:
								throw new Exception("Method POST is required. Report about this issue by visiting https://pushalot.codeplex.com/workitem/list/basic.");
							case HttpStatusCode.NotAcceptable:
								throw new Exception("Message throttle limit hit. Check result information Description field for information which limit was exceeded. See limits (https://pushalot.com/api#limits) to learn more about what limits are enforced.");
							case HttpStatusCode.Gone:
								throw new Exception("The AuthorizationToken is no longer valid and no more messages should be ever sent again using that token.");
							case HttpStatusCode.InternalServerError:
								throw new Exception("Something is broken. Please contact Pushalot.com (https://pushalot.com/support) so we can investigate.");
							case HttpStatusCode.ServiceUnavailable:
								throw new Exception("Pushalot.com servers are currently overloaded with requests. Try again later.");
							default:
								throw new Exception("Unknown error.");
						}
					}
				}
			}
		}

		private static string GetJsonText(PushalotMessage message, PushalotAuthorizationToken authorizationToken)
		{
			var jsonDictionary = new Dictionary<string, object>();

			jsonDictionary.Add(JSON_FIELD_NAME_AUTHORIZATION_TOKEN, authorizationToken.Token);

			if (!string.IsNullOrEmpty(message.Title))
			{
				jsonDictionary.Add(JSON_FIELD_NAME_TITLE, message.Title);
			}

			jsonDictionary.Add(JSON_FIELD_NAME_BODY, message.Body);

			if (message.Link != null)
			{
				if (!string.IsNullOrEmpty(message.Link.Title))
				{
					jsonDictionary.Add(JSON_FIELD_NAME_LINK_TITLE, message.Link.Title);
				}

				jsonDictionary.Add(JSON_FIELD_NAME_LINK, message.Link.Link.AbsoluteUri);
			}

			jsonDictionary.Add(JSON_FIELD_NAME_IS_IMPORTANT, message.IsImportant);
			jsonDictionary.Add(JSON_FIELD_NAME_IS_SILENT, message.IsSilent);

			if (message.Image != null)
			{
				jsonDictionary.Add(JSON_FIELD_NAME_IMAGE, message.Image.Image.AbsoluteUri);
			}

			if (!string.IsNullOrEmpty(message.Source))
			{
				jsonDictionary.Add(JSON_FIELD_NAME_SOURCE, message.Source);
			}

			if (message.TimeToLive.HasValue)
			{
				jsonDictionary.Add(JSON_FIELD_NAME_TIME_TO_LIVE, message.TimeToLive.Value);
			}

			string jsonText = JsonConvert.SerializeObject(jsonDictionary);

			return jsonText;
		}
	}

	public class PushalotClient<T> : PushalotClient
	{
		private Lazy<ILookup<T, PushalotAuthorizationToken>> specialSubscribersLookup;
		private HashSet<Tuple<T, PushalotAuthorizationToken>> specialSubscribersSet;

		public PushalotClient()
			: base()
		{
			specialSubscribersSet = new HashSet<Tuple<T, PushalotAuthorizationToken>>();

			ResetSpecialSubscribersLookup();
		}

		public async Task SendMessage(PushalotMessage message, params T[] tags)
		{
			await base.SendMessage(message);

			foreach (var tag in tags)
			{
				foreach (var specialSubscriber in specialSubscribersLookup.Value[tag])
				{
					await SendMessage(message, specialSubscriber);
				}
			}
		}

		public bool Subscribe(T tag, PushalotAuthorizationToken authorizationToken)
		{
			return specialSubscribersSet.Add(new Tuple<T, PushalotAuthorizationToken>(tag, authorizationToken));
		}

		public bool Unsubscribe(T tag, PushalotAuthorizationToken authorizationToken)
		{
			return specialSubscribersSet.Remove(new Tuple<T, PushalotAuthorizationToken>(tag, authorizationToken));
		}

		private ILookup<T, PushalotAuthorizationToken> InitializeSpecialSubscribersLookup()
		{
			return specialSubscribersSet.ToLookup(item => item.Item1, item => item.Item2);
		}

		private void ResetSpecialSubscribersLookup()
		{
			specialSubscribersLookup = new Lazy<ILookup<T, PushalotAuthorizationToken>>(InitializeSpecialSubscribersLookup);
		}
	}
}