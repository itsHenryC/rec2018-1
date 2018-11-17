
using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

// For more information about this template visit http://aka.ms/azurebots-csharp-basic
[Serializable]
public class AIDialog : IDialog<object>
{
    static string[] options = {"Nearest food supplier", "Report an incident", "I need to talk to someone..."};
    static string[] restuarants ={"Dummy Restuarant: 123 Test St.", "Test Restuarant: 123 Trial St.", "Fake Restuarant: 321 Fake St."};
    string name = "";

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
        var activity = await argument;
        if (activity.Text == "No" || activity.Text == "NO" || activity.Text == "no" || activity.Text == "nO" ){
            await context.PostAsync("Have a great day!"); 
        }
        else{
        ShowOptions(context);
        }
    }

    private void ShowOptions(IDialogContext context) {
        PromptDialog.Choice<string>(
                context,
                AfterChoiceReceivedAsync,
                new []{options[0], options[1], options[2]},
                "How can I help you?",
                "Ops, I didn't get that...",
                5,
                promptStyle: PromptStyle.Auto);
    }

    public virtual async Task AfterChoiceReceivedAsync(IDialogContext context, IAwaitable<string> argument)
    {
        var response = await argument;
        switch (response)
        {
            case "Nearest food supplier":
                    await context.PostAsync("Where are you located?"); 
                    context.Wait(GetFood);
                break;
            case "Report an incident":
                    await context.PostAsync("Please describe your incident. (All information is confidential)");
                    context.Wait(GetIncident); 
                break;
            case "I need to talk to someone...":
                await context.PostAsync("Of course " + name + ", here are some available resources.");   
                await context.PostAsync("Suicide Prevention Resource Center: https://www.sprc.org/");                  
                await context.PostAsync("National Suicide Prevention Lifeline: 1-800-273-8255");               
                break; 
            default:
                await context.PostAsync("Invalid choice.");
                break;
        }
    }

    public async Task GetFood(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
        var food = await argument;
        await context.PostAsync("The nearest food location near " + food.Text + " is " + restuarants[0]);
        await context.PostAsync("Would you like to continue?");
        context.Wait(MessageReceivedAsync);
    }

    public async Task GetIncident(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
        var incident = await argument;
        await context.PostAsync("Your incident has been logged. Your report ID is: 12345");
        await context.PostAsync("Would you like to continue?");
        context.Wait(MessageReceivedAsync);
    }
}
