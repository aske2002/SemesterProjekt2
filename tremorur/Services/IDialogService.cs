namespace tremorur.Services
{
	public interface IDialogService
	{
		Task DisplayAlertAsync(string title, string message, string cancel);

		Task DisplayAlertAsync(string title, string message, string accept, string cancel);
		 Task<string> DisplayPromptAsync(string title, string message, string accept, string cancel);

	}
}
