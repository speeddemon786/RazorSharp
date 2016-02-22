function LoadCharts() {

    Morris.Area({
        element: 'page-views',
        data: Chart('area'),
        xkey: 'label',
        ykeys: ['value'],
        labels: ['Views'],
        resize: true
    });

    Morris.Bar({
        element: 'top-pages',
        data: Chart('bar'),
        xkey: 'label',
        ykeys: ['value'],
        labels: ['Views'],
        resize: true
    });

    Morris.Donut({
        element: 'top-browsers',
        data: Chart('donut1'),
        resize: true
    });

    Morris.Donut({
        element: 'top-os',
        data: Chart('donut2'),
        resize: true
    });

}


function Chart(chartType) {
    var data = "";
    $.ajax({
        type: 'GET',
        url: "/Json/Charts",
        dataType: 'json',
        async: false,
        contentType: "application/json; charset=utf-8",
        data: { 'chart': chartType },
        success: function (result) {
            data = result;
        },
        error: function (xhr, status, error) {
            alert(error);
        }
    });
    return data;
}