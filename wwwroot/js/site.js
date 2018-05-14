// Write your Javascript code.
$(function(){
    $.ajax({
        method: 'POST',
        url: 'Schedule/SetTimeZoneCookie',
        data: { time: new Date()}
    });

    $.ajax({
        method: 'POST',
        url: 'Schedule/SetCultureCookie',
        data: { time: new Date()}
    });
});