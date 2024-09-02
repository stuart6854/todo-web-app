using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using TodoWebApp.Models.Entities;
using TodoWebApp.Models.Models;

namespace TodoWebApp.Web.Components.Pages.Tasks;

public partial class IndexTasks
{
    [Inject]
    private ApiClient ApiClient { get; set; }

    public List<TaskModel> Tasks { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadTasks();
    }

    protected async Task LoadTasks()
    {
        var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>("/api/Tasks");
        if (res is { Success: true })
        {
            Tasks = JsonConvert.DeserializeObject<List<TaskModel>>(res.Data.ToString());
            StateHasChanged();
        }
    }
}