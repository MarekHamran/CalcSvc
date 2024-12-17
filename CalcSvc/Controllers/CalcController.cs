using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text.Json.Serialization;

namespace CalcSvc.Controllers
{
	public class CalcResponse
	{
		[JsonPropertyName("computed_value")]
		public decimal ComputedValue { get; set; }

		[JsonPropertyName("input_value")]
		public decimal InputValue { get; set; }

		[JsonPropertyName("previous_value")]
		public decimal? PreviousValue { get; set; }
	}

	public class CalcRequest
	{
		[JsonPropertyName("input")]
		public decimal Input { get; set; }
	}

	[ApiController]
	[Route("[controller]")]
	public class CalcController : ControllerBase
	{
		private static readonly int defaultValue = 2;
		private static readonly int maxAgeInSeconds = 15;

		private readonly ILogger<CalcController> _logger;
		private readonly IStorage _storage;
		private readonly IMessenger _messenger;

		public CalcController(ILogger<CalcController> logger, IStorage storage, IMessenger messenger)
		{
			_logger = logger;
			_storage = storage;
			_messenger = messenger;
		}

		[Route("/Calculation/{key}")]
		[HttpPost]
		public ActionResult<CalcResponse> Calculation([FromRoute] int key, [FromBody] CalcRequest request)
		{
			if (request.Input <= 2)
				return BadRequest("Input value must be greater than 2");

			decimal value = defaultValue;

			StorageEntry? obj = _storage.Get(key) as StorageEntry;

			if (obj != null && (DateTime.UtcNow - obj.Age).TotalSeconds < defaultValue)
			{
				value = Convert.ToDecimal(Math.Cbrt(Math.Log(Convert.ToDouble(request.Input / obj.Value))));
				_messenger.SendMessageAsync(value.ToString());
			}

			_storage.Put(key, new StorageEntry(value));
			return new CalcResponse { ComputedValue = value, InputValue = request.Input, PreviousValue = (obj != null) ? obj.Value : null };
		}
	}
}
