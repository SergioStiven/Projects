$(document).ready(
    function () {
        $("#ms-countdown").countdown("2017/04/15", function (l) {
            $(this).html(l.strftime(
                '<ul class="coming-date coming-date-black">'+
                   '<li style="font-size:25px;">%D <span>Dias</span></li>' +
                   '<li style="font-size:25px;" class="colon">:</li>' +
                   '<li style="font-size:25px;">%H <span>Horas</span></li>' +
                   '<li style="font-size:25px;" class="colon">:</li>' +
                   '<li style="font-size:25px;">%M <span>Minutos</span></li>' +
                   '<li style="font-size:25px;" class="colon">:</li>' +
                   '<li style="font-size:25px;">%S <span>Seg</span></li>' +
               '</ul>'))
        })
    });