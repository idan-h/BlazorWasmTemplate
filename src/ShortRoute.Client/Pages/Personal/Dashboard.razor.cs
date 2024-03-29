﻿using System.Globalization;
using ShortRoute.Client.Infrastructure.ApiClient;
using ShortRoute.Client.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ShortRoute.Client.Pages.Personal;

public partial class Dashboard
{
    [Parameter]
    public int ProductCount { get; set; }
    [Parameter]
    public int BrandCount { get; set; }
    [Parameter]
    public int UserCount { get; set; }
    [Parameter]
    public int RoleCount { get; set; }

    private readonly string[] _dataEnterBarChartXAxisLabels = DateTimeFormatInfo.CurrentInfo.AbbreviatedMonthNames;
    private readonly List<ChartSeries> _dataEnterBarChartSeries = new();
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();

        _loaded = true;
    }

    private async Task LoadDataAsync()
    {
        //if (await ApiHelper.ExecuteClientCall(
        //        () => DashboardClient.GetAsync(),
        //        Snackbar)
        //    is StatsDto statsDto)
        //{
        //    ProductCount = statsDto.ProductCount;
        //    BrandCount = statsDto.BrandCount;
        //    UserCount = statsDto.UserCount;
        //    RoleCount = statsDto.RoleCount;
        //    foreach (var item in statsDto.DataEnterBarChart)
        //    {
        //        _dataEnterBarChartSeries
        //            .RemoveAll(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
        //        _dataEnterBarChartSeries.Add(new MudBlazor.ChartSeries { Name = item.Name, Data = item.Data?.ToArray() });
        //    }
        //}

        ProductCount = 1;
        BrandCount = 2;
        UserCount = 3;
        RoleCount = 4;
        _dataEnterBarChartSeries.Add(new ChartSeries { Name = "test", Data = new[] { 1.0, 2, 3 } });
    }
}
