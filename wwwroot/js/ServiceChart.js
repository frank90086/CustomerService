
//Chart
var barChart, timer3000;
var barOptions = {
    legend: {
        display: false
    }
};
// var barctx = document.getElementById('chartjs-Appraise').getContext('2d');
var rFactor = function () {
    return Math.round(Math.random() * 10);
};

var dt = new Date();
function setDate(setdate, value) {
    var setdt = new Date();
    setdt.setDate(setdate.getDate() + value);
    return (setdt.getMonth() + 1) + '/' + setdt.getDate();
}
// var verypoor = $('#verypoorCount').text();
// var poor = $('#poorCount').text();
// var ok = $('#okCount').text();
// var good = $('#goodCount').text();
// var verygood = $('#verygoodCount').text();

// var barData = {
//     labels: [setDate(dt, -6), setDate(dt, -5), setDate(dt, -4), setDate(dt, -3), setDate(dt, -2), setDate(dt, -1), 'Today'],
//     datasets: [{
//         backgroundColor: '#f05050',
//         borderColor: '#f05050',
//         data: [rFactor(), rFactor(), rFactor(), rFactor(), rFactor(), rFactor(), verypoor]
//     }, {
//         backgroundColor: '#ff902b',
//         borderColor: '#ff902b',
//         data: [rFactor(), rFactor(), rFactor(), rFactor(), rFactor(), rFactor(), poor]
//     }, {
//         backgroundColor: '#23b7e5',
//         borderColor: '#23b7e5',
//         data: [rFactor(), rFactor(), rFactor(), rFactor(), rFactor(), rFactor(), ok]
//     }, {
//         backgroundColor: '#5d9cec',
//         borderColor: '#5d9cec',
//         data: [rFactor(), rFactor(), rFactor(), rFactor(), rFactor(), rFactor(), good]
//     }, {
//         backgroundColor: '#27c24c',
//         borderColor: '#27c24c',
//         data: [rFactor(), rFactor(), rFactor(), rFactor(), rFactor(), rFactor(), verygood]
//     }]
// };

var wait = $('#waitCount').text();
var handling = $('#handlingCount').text();
var complete = $('#completeCount').text();
var runaway = $('#runawayCount').text();
var data = [{
    "label": "wait (" + wait + ")",
    "color": "#f05050",
    "data": wait
}, {
    "label": "handling (" + handling + ")",
    "color": "#37bc9b",
    "data": handling
}, {
    "label": "complete (" + complete + ")",
    "color": "#5d9cec",
    "data": complete
}, {
    "label": "runaway (" + runaway + ")",
    "color": "#131e26",
    "data": runaway
}];
var options = {
    series: {
        pie: {
            show: true,
            innerRadius: 0,
            label: {
                show: true,
                radius: 0.8,
                formatter: function (label, series) {
                    return '<div class="flot-pie-label">' +
                        Math.round(series.percent) +
                        '%</div>';
                },
                background: {
                    opacity: 1,
                    color: '#ffffff'
                }
            }
        }
    }
};
var chart = $('.chart-pie');
//Chart


function autoReload() {
    $.ajax({
        url: '_customerList',
        type: 'post',
        cache: false,
        async: false,
        data: {},
        success: function (result) {
            $('#updateList').html(result);
            clearInterval(timer3000);
            timer3000 = setInterval(updateTime, 1000);
        }
    });

    $.ajax({
        url: '_itList',
        type: 'post',
        cache: false,
        async: false,
        data: {},
        success: function (result) {
            $('#updateitList').html(result);
            clearInterval(timer3000);
            timer3000 = setInterval(updateTime, 1000);
        }
    });

    $.ajax({
        url: '_calculationChart',
        type: 'post',
        cache: false,
        async: false,
        data: {},
        success: function (result) {
            data = [{
                "label": "wait (" + result.waitcount + ")",
                "color": "#f05050",
                "data": result.waitcount
            }, {
                "label": "handling (" + result.handlingcount + ")",
                "color": "#37bc9b",
                "data": result.handlingcount
            }, {
                "label": "complete (" + result.completecount + ")",
                "color": "#5d9cec",
                "data": result.completecount
            }, {
                "label": "runaway (" + result.runawaycount + ")",
                "color": "#131e26",
                "data": result.runawaycount
            }];
            UpdateChart(data);
        }
    });

    // $.ajax({
    //     url: '_calculationAppraise',
    //     type: 'post',
    //     cache: false,
    //     async: false,
    //     data: {},
    //     success: function (result) {
    //         var labels = barData["labels"];
    //         var oldverypoor = barData["datasets"][0]["data"];
    //         var oldpoor = barData["datasets"][1]["data"];
    //         var oldok = barData["datasets"][2]["data"];
    //         var oldgood = barData["datasets"][3]["data"];
    //         var oldverygood = barData["datasets"][4]["data"];
    //         var newverypoor = result.verypoorcount;
    //         var newpoor = result.poorcount;
    //         var newok = result.okcount;
    //         var newgood = result.goodcount;
    //         var newverygood = result.verygoodcount;
    //         oldverypoor[6] = newverypoor;
    //         oldpoor[6] = newpoor;
    //         oldok[6] = newok;
    //         oldgood[6] = newgood;
    //         oldverygood[6] = newverygood;
    //         UpdateAppraise(barData, barOptions);
    //     }
    // });
}

// CHART PIE
// ----------------------------------- 
function UpdateChart(data) {
    $.plot(chart, data, options);
}

// Bar chart
// -----------------------------------
// function UpdateAppraise(data, option) {
//     barChart.destroy();
//     barChart = new Chart(barctx, {
//         data: data,
//         type: 'bar',
//         options: option
//     });
// }

// Timer
// -----------------------------------
function updateTime() {
    var bodyelement = $('.databody').find('td.timeelement');
    $.each(bodyelement, function (index, value) {
        var check = $(this).parent().find('td.statuselement');
        var _parent = $(this).closest('tr');
        var checkText = check[0].innerText;
        if (checkText === 'Handling') {
            _parent.addClass('bg-success').animate({ color: 'white', });
        }
        if (checkText !== 'Wait') {
            return;
        }
        var time = value.innerText;
        var addTime = moment(time, 'YYYY/MM/DD h:mm:ss').toDate();
        var stamp = moment(new Date(), 'YYYY/MM/DD h:mm:ss').diff(addTime, 'seconds');
        var timer = moment.duration((stamp), 'seconds').format('h:mm:ss');
        $(this).next('td.waittimeelement').html(timer);
        if ((stamp) > '20') {
            _parent.addClass('bg-danger').animate({ color: 'white', });
        }
        else if ((stamp) > '10') {
            _parent.addClass('bg-warning').animate({ color: 'white', });
        }
    });
}

$(function () {
    if (chart.length)
        $.plot(chart, data, options);
});

// $(function () {
//     if (typeof Chart === 'undefined') return;

//     barChart = new Chart(barctx, {
//         data: barData,
//         type: 'bar',
//         options: barOptions
//     });
// });

$(function () {
    timer3000 = setInterval(updateTime, 1000);
});

(function (window, document, $, undefined) {

    $(function () {

        if (typeof Chartist === 'undefined') return;

        // Bar bipolar
        // ----------------------------------- 
        var options1 = {
            high: 10,
            low: -10,
            height: 280,
            axisX: {
                labelInterpolationFnc: function (value, index) {
                    return index % 2 === 0 ? value + '(S)' : null;
                }
            }
        };

        // Bar Horizontal
        // ----------------------------------- 
        // new Chartist.Bar('#ct-bar2', {
        //     labels: ['AVG接線時間', 'AVG完成時間'],
        //     series: [
        //         [(rFactor() + 1) * 3, (rFactor() + 1) * 12]
        //     ]
        // }, {
        //         seriesBarDistance: 10,
        //         reverseData: true,
        //         horizontalBars: true,
        //         height: 280,
        //         axisX: {
        //             labelInterpolationFnc: function (value, index) {
        //                 return index % 5 === 0 ? value + '(S)' : null;
        //             }
        //         },
        //         axisY: {
        //             offset: 80
        //         }
        //     });
    });

})(window, document, window.jQuery);

