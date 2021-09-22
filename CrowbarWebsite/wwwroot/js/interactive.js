/*
 * INTERACTIVE.JS
 * ==============
 * CODE FOR LIVE INTERACTIVITY WITH CROWBAR MAP
 * */
var xINTERACTIVE = {

    MARKERS_SCAMS: [],
    MARKERS_RCAMS: [],
    MARKERS_AREA: [],
    ICONS: {
        "icon_redlight": L.icon({
            iconUrl: './lib/art/camera-redlight.png',
            iconSize: [26, 26],
            iconAnchor: [13.5, 26],
            popupAnchor: [0, -26]
        }),
        "icon_staticcamera": L.icon({
            iconUrl: './lib/art/camera-static.png',
            iconSize: [26, 26],
            iconAnchor: [13.5, 26],
            popupAnchor: [0, -26]
        })
    },


    FN_INITIALISE: function () {
        this.MAP = L.map('ljsmap').setView([-41.2177074, 178.1841412], 5.96);
        L.tileLayer('https://a.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '<a href="https://www.openstreetmap.org/copyright">© OpenStreetMap contributors</a>',
            maxZoom: 18,
            tileSize: 512,
            zoomOffset: -1
        }).addTo(this.MAP);
        this.FN_FETCHCAMERAS();
    },

    FN_TOGGLE_SCAMS: function () {
        if ($("#CID_interactive_scams").prop("checked")) {
            this.MARKERS_SCAMS.forEach(function (x) {
                x.addTo(this.MAP);
            })
        }
        else {
            this.MARKERS_SCAMS.forEach(function (x) {
                MAP.removeLayer(x);
            })
        }
    },
    FN_TOGGLE_RCAMS: function () {
        SubmitPref('@UserPref.SNAMES[UserPrefProps.SHOW_STATIC_CAMERAS]', $("#xui_cfg-rcams").prop("checked"));
    },
    FN_FETCHCAMERAS: function (filter) {
        $.ajax({
            url: '/Home/GetCameraData?f1=rs',
            type: 'GET',
            dataType: "json",
            cache: false,
            success: function (data) {
                //do stuff;
                MARKERS_SCAMS = [];
                MARKERS_RCAMS = [];
                console.log(data);
            },
            error: function () {
                console.log("no cams");
            }
        });
    }




};
/*
//Reload Map
function SubmitPref(pref, val) {
    $.ajax({
        url: '/Home/SetSessionPrefs?pref=' + pref + '&value=' + val,
        type: 'POST',
        dataType: "json",
        cache: false,
        success: function (data) {
            //do stuff;
            $('#indexdock_map').load('/Home/Map');
        },
        error: function () {
            console.log("no display");
        }
    });
}*/

let INTERACTIVE = xINTERACTIVE;


