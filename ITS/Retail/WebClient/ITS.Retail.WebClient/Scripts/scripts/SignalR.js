var SignalR;
SignalR = (function () {
    return {
        StartApplicationUsersHub: function () {
            var applicationUsersHub;
            applicationUsersHub = $.connection.applicationUsersHub;
            applicationUsersHub.client.assertUsers = function (usersInfo) {
                $('#connectedUsers').html(usersInfo);
            };
            $.connection.hub.start().done(function (e) {
                console.log(e);
                return applicationUsersHub.server.send('', '');
            }).fail(function (error) {
                return console.log(error);
            });
            return;
        }
    };
})();
$(function () {
    SignalR.StartApplicationUsersHub();
    return window.onbeforeunload = function (e) {
        $.connection.hub.stop();
    };
});
