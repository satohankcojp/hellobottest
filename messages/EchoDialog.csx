using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;

// For more information about this template visit http://aka.ms/azurebots-csharp-basic
[Serializable]
public class EchoDialog : IDialog<object>
{
    protected int count = 1;

    public Task StartAsync(IDialogContext context)
    {
        try
        {
            context.Wait(MessageReceivedAsync);
        }
        catch (OperationCanceledException error)
        {
            return Task.FromCanceled(error.CancellationToken);
        }
        catch (Exception error)
        {
            return Task.FromException(error);
        }

        return Task.CompletedTask;
    }

    public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
        var message = await argument;
        if (message.Text == "reset")
        {
            PromptDialog.Confirm(
                context,
                AfterResetAsync,
                "Are you sure you want to reset the count?",
                "Didn't get that!",
                promptStyle: PromptStyle.Auto);
        }
        else if (message.Text == "time")
        {
            DateTime time = DateTime.Now;
            await context.PostAsync($"{time.Hour + 9}:{time.Minute}");
            context.Wait(MessageReceivedAsync);
        }
        else if (Regex.IsMatch(message.Text, @"\d\d\d\d\d\d\d"))
        {
            /*try
            {
                HttpClient client = new HttpClient();
                var result = await client.GetAsync($"http://zipcloud.ibsnet.co.jp/api/search?zipcode={message.Text}");
                if (result)
                {
                    var address = JsonConvert.DeserializeObject<Address>(await result.Content.ReadAsStringAsync());
                    await context.PostAsync(address.address1);
                }
                else
                {
                    await context.PostAsync("ADDRESS ERROR");
                }

                context.Wait(MessageReceivedAsync);
            }
            catch (e)
            {
                await context.PostAsync($"ADDRESS ERROR");
                context.Wait(MessageReceivedAsync);
            }*/

        }
        else
        {
            await context.PostAsync($"{this.count++}: You said {message.Text}");
            context.Wait(MessageReceivedAsync);
        }
    }

    public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
    {
        var confirm = await argument;
        if (confirm)
        {
            this.count = 1;
            await context.PostAsync("Reset count.");
        }
        else
        {
            await context.PostAsync("Did not reset count.");
        }
        context.Wait(MessageReceivedAsync);
    }
    public class Address
    {
        public string zipcode { get; set; }
        public string address1 { get; set; }
    }

}