var Contact = {
    Init: function () {
        Contact.InitializeMap();
    },
    InitializeMap: function () {

        if (typeof (companyLongtitude) != typeof (undefined) && typeof (companyLatitute) != typeof (undefined)) {
            var map = new GMaps({
                div: '#map-canvas',
                lat: companyLatitute,
                lng: companyLongtitude
            });

            map.addMarker({
                lat: companyLatitute,
                lng: companyLongtitude,
                title: companyName,
                infoWindow: {
                    content: mapContent
                }
            });
        }
        
    },
    ClearForm: function(){
        FullName.Clear();
        Email.Clear();
        Subject.Clear();
        Message.Clear();
    }
};

