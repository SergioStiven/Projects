var data = {
    labels: [
      "Candidatos",
      "Representantes",
      "Grupos",
      "Anunciantes"
    ],
    datasets: [{
        data: [300, 50, 200, 100],
        backgroundColor: [
          "#FF6384",
          "#36A2EB",
          "#FFCE56",
          "#78FF64"
        ],
        hoverBackgroundColor: [
          "#FF6384",
          "#36A2EB",
          "#FFCE56",
          "#78FF64"
        ],
        borderWidth: 1
    }]
};

var options = {
    responsive: true,
    animation: {
        animateRotate: true,
        animateScale: true
    },
    cutoutPercentage: 55,
    legend: false,
    legendCallback: function (chart) {
        var text = [];
        text.push('<ul class="' + chart.id + '-legend">');
        for (var i = 0; i < chart.data.datasets[0].data.length; i++) {
            text.push('<li><span style="background-color:' + chart.data.datasets[0].backgroundColor[i] + '">');
            if (chart.data.labels[i]) {
                text.push(chart.data.labels[i]);
            }
            text.push('</span></li>');
        }
        text.push('</ul>');
        return text.join("");
    },
    tooltips: {
        custom: function (tooltip) {
            //tooltip.x = 0;
            //tooltip.y = 0;
        },
        mode: 'single',
        callbacks: {
            label: function (tooltipItems, data) {
                var sum = data.datasets[0].data.reduce(add, 0);
                function add(a, b) {
                    return a + b;
                }

                return parseInt((data.datasets[0].data[tooltipItems.index] / sum * 100), 10) + ' %';
            },
            beforeLabel: function (tooltipItems, data) {
                return data.datasets[0].data[tooltipItems.index] + ' hrs';
            }
        }
    }
}
var ctx = $("#myChart");
var myChart = new Chart(ctx, {
    type: 'doughnut',
    data: data,
    options: options
});
$("#chartjs-legend").html(myChart.generateLegend());
$("#chartjs-legend").on('click', "li", function () {
    myChart.data.datasets[0].data[$(this).index()] += 50;
    myChart.update();
    console.log('legend: ' + data.datasets[0].data[$(this).index()]);
});
$('#myChart').on('click', function (evt) {
    //var activePoints = myChart.getElementsAtEvent(evt);
    //var firstPoint = activePoints[0];
    //if (firstPoint !== undefined) {
    //    console.log('canvas: ' + data.datasets[firstPoint._datasetIndex].data[firstPoint._index]);
    //} else {
    //    myChart.data.labels.push("New");
    //    myChart.data.datasets[0].data.push(100);
    //    myChart.data.datasets[0].backgroundColor.push("red");
    //    myChart.options.animation.animateRotate = false;
    //    myChart.options.animation.animateScale = false;
    //    myChart.update();
    //    $("#chartjs-legend").html(myChart.generateLegend());
    //}
});