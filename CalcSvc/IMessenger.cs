namespace CalcSvc
{
	public interface IMessenger
	{
		Task SendMessageAsync(string message);
		Task<string?> GetMessageAsync(CancellationToken cancellationToken);
	}
}
