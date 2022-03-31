$(() => {
  var var_mapoptions;
  var var_map;
  new WOW().init();

  $('#CityName').mdbAutocomplete({
    data: cities,

  }).change(e => {
    var ipt = $(e.target);
    if (cities.includes(ipt.val())) {
      lookupUni(ipt.val());
    }
  });
  $('.mdb-autocomplete-wrap').on("mousedown", "li", (e) => {
    lookupUni($(e.target).text());
  });


});
function lookupUni(cityName, lan, fields) {
  map.entities.remove(window.cityPolygon)
  map.entities.remove(window.pins)
  Microsoft.Maps.loadModule(['Microsoft.Maps.SpatialDataService', 'Microsoft.Maps.Search'], function () {
    var searchManager = new Microsoft.Maps.Search.SearchManager(map);
    var geocodeRequest = {
      where: cityName,
      callback: function (geocodeResult) {
        if (geocodeResult && geocodeResult.results && geocodeResult.results.length > 0) {
          map.setView({ bounds: geocodeResult.results[0].bestView });
          var geoDataRequestOptions = {
            entityType: 'PopulatedPlace',
            getAllPolygons: true
          };
          setTimeout(() => {
            map.setView({ zoom: 10 });
            setTimeout(() => {
              unisearch(cityName);

            }, 200);
          }, 200);
          showCityOnBtn(cityName);
          //Use the GeoData API manager to get the boundary of New York City
          Microsoft.Maps.SpatialDataService.GeoDataAPIManager.getBoundary(geocodeResult.results[0].location, geoDataRequestOptions, map, function (data) {
            if (data.results && data.results.length > 0) {
              window.cityPolygon = (data.results[0].Polygons);
              map.entities.push(cityPolygon);
            }
          }, null, function errCallback(networkStatus, statusMessage) {
            console.log(networkStatus);
            console.log(statusMessage);
          });
        }
      },
    };
    searchManager.geocode(geocodeRequest);
  });

}
function createPin(r, uni, tpins, rslt) {
  let i = tpins.length;
  //Create a pushpin for each result.
  let loc = new Microsoft.Maps.Location(r.lat, r.lon)
  let pin = new Microsoft.Maps.Pushpin(loc, { text: uni.Acronym.length <= 7 ? uni.Acronym : i.toString() });
  pin.metadata = {
    uni: uni,
    result: r,
    rslt: rslt
  };
  tpins.push(pin);
  pin.setOptions({ enableHoverStyle: true, enableClickedStyle: false });
  Microsoft.Maps.Events.addHandler(pin, 'click', function (args) {
    if (infobox.pin == args.target) {
      infobox.setOptions({ visible: !infobox.getVisible() });
    }
    else {
      infobox.setOptions({
        location: args.target.getLocation(),
        title: args.target.metadata.uni.UniName,
        description: "Rank: " + args.target.metadata.uni.Rank,
        visible: true
      });
      infobox.pin = args.target;
    }
  });
}
function FetchInfo(unistack) {
  let u = unistack.pop();
  if (!u) return;

  $.ajax({
    url: 'https://nominatim.openstreetmap.org/search.php?q=' + u.UniName.replace(/ /g, "+") + '&format=json&addressdetails=1&limit=50&accept-language=de',

    dataType: 'json', // Notice! JSONP <-- P (lowercase)
    success: function (r) {
      if (r && r.length > 0) {
        var d = true;
        for (var i = 0; i < r.length; i++) {
          if ((!!u.City & !!r[i].address.city && r[i].address.city.toLowerCase() == u.City.toLowerCase()) ||
            (!!u.City & !!r[i].address.town && r[i].address.town.toLowerCase() == u.City.toLowerCase())) {
            d = false;
            map.entities.remove(window.pins)
            createPin(r[i], u, pins, r);
            map.entities.push(pins);
            break;
          }
        }
        if (d)
          (window.notUse || (window.notUse = [])).push({ u, r });
        //Add the pins to the map

      }
      FetchInfo(unistack)
    },
    error: function (e, b, c) {
      FetchInfo(unistack)
    }
  });
}
function unisearch(cityName) {




  window.pins = []
  let uniselected = []
  joinUni.forEach(uni => {
    if (uni.City == cityName) {
      uniselected.push(uni)
    }
  });
  FetchInfo(uniselected);

}

function updateuilist() {
  let btn = $('#btnCompare');
  btn.attr('disabled', 'disabled');
  for (var i = 0; i < 5; i++) {
    $('#cmplst' + i.toString()).text("").parent().addClass("d-none");
  }
  for (var i = 0; i < Comparisonlist.length; i++) {
    let item = Comparisonlist[i];
    $('#cmplst' + i.toString()).text(item.uni.UniName).parent().removeClass("d-none");
    btn.attr('disabled', null);
  }
}



function loadMapScenario() {
  $('#btnaddcomparison').click((e) => {
    $('#UniInfoModal').modal('hide');
    AddToComparisonlist($(e.target).data('uni'));
  });
  $('#btnCompare').click(() => {
    let ids = Comparisonlist[0].uni.UniId;
    for (let i = 1; i < Comparisonlist.length; i++) {
      ids += "," + Comparisonlist[i].uni.UniId;
    }
    location.href = "comparison.html?ids=" + ids;
  });
  let cityonMapSelect = $('#cityonMapSelect');
  cityonMapSelect.click(e => {
    $('#CityName').val(cityonMapSelect.find('span').text()).change()
  });
  window.showCityOnBtn = function (cityName) {
    cityonMapSelect.find('span').text(cityName);
  }

  Microsoft.Maps.ConfigurableMap.createFromConfig(document.getElementById('map-container'), 'style/configmap2.json', false, null, successCallback, errorCallback);
  function successCallback(mapObj) {
    window.map = mapObj;
    Microsoft.Maps.Events.addHandler(map, 'click', function (e) {
      $.ajax({
        url: 'https://nominatim.openstreetmap.org/reverse?lat=' + e.location.latitude + '&lon=' + e.location.longitude + '&zoom=18&format=json&addressdetails=1&accept-language=de',

        dataType: 'json', // Notice! JSONP <-- P (lowercase)
        success: function (r) {
          if (r && r.address && r.address.city) {

            showCityOnBtn(r.address.city);


          }
        },
        error: function (e, b, c) {
        }
      });
    });
    window.infobox = new Microsoft.Maps.Infobox(mapObj.getCenter(), {
      visible: false, autoAlignment: true, actions: [
        {
          label: 'Read more', eventHandler: function (e) {
            var modal = $('#UniInfoModal').modal('show');
            modal.find('.heading.lead').text(infobox.pin.metadata.uni.UniName);
            modal.find('#UniAddress').text(infobox.pin.metadata.uni.Address);
            modal.find('#tel span').text(infobox.pin.metadata.uni.Tel);
            modal.find('#founded span:nth-child(2)').text(infobox.pin.metadata.uni.Founded);
            modal.find('#TotalStd span').text(infobox.pin.metadata.uni.other1[0].total_number_of_students);
            if (sessionStorage.getItem('degree') == 1)
              modal.find('#TuitionIntStdnB span').text(infobox.pin.metadata.uni.TuitionIntStdnB);
            else if (sessionStorage.getItem('degree') == 2)
              modal.find('#TuitionIntStdnB span').text(infobox.pin.metadata.uni.TuitionIntStdnM);
            else
              modal.find('#TuitionIntStdnB span').text(`${infobox.pin.metadata.uni.TuitionIntStdnB} (B),${infobox.pin.metadata.uni.TuitionIntStdnM} (M)`);
            modal.find('#CampusSetting span').text(infobox.pin.metadata.uni.CampusSetting);
            modal.find('#ControlType span').text(infobox.pin.metadata.uni.ControlType);
            modal.find('#SelectionType span').text(infobox.pin.metadata.uni.SelectionType);
            var a=uinpoint.filter(a=>a.uni.UniId==infobox.pin.metadata.uni.UniId)
            let dataDounat=!a || !a.length?[]:
              a[0].dparts.map(a=>[a.department_name,
                (sessionStorage.getItem('degree') == 1? 
                  a.graduations_in_appropriate_time_undergraduate_degrees:
                  (sessionStorage.getItem('degree') == 2?
                    a.graduations_in_appropriate_time_masters:
                    (a.graduations_in_appropriate_time_undergraduate_degrees+
                    a.graduations_in_appropriate_time_masters)/2
              ))]);
              (window.dounatchart||{ destroy: () => { } }).destroy();
              window.dounatchart = c3.generate({
                bindto: '#dounatChart1',
                data: {
                    columns: dataDounat,
                    type : 'donut',
                },
                donut: {
                    title: "Departments"
                },
                legend: {
                    show: false
                }
            });
            $('#btnaddcomparison').data('uni', infobox.pin.metadata);
            //AddToComparisonlist(infobox.pin.metadata);
          }
        }
      ]
    });
    window.infobox.setMap(mapObj);
    window.pins = [];
    window.Comparisonlist = [];
    window.AddToComparisonlist = function (m) {
      if (Comparisonlist.filter(item => item.uni == m.uni).length > 0)
        return;
      Comparisonlist.push({ uni: m.uni, result: m.result });
      if (Comparisonlist.length > 5)
        Comparisonlist.shift();
      updateuilist();
    }
    window.removeToComparisonlist = function (i) {
      if (i < Comparisonlist.length)
        Comparisonlist.splice(i, 1);
      updateuilist();
    }
    updateuilist()
    setTimeout(() => {
      var bounds = Microsoft.Maps.LocationRect.fromLocations(new Microsoft.Maps.Location(47.15630905857346, 15.408189134456976),
        new Microsoft.Maps.Location(55.85630905857346, 5.653838560219752));
      map.setOptions({
        maxZoom: 20,
        minZoom: 5,
        maxBounds: bounds
      });

      map.setView({
        mapTypeId: Microsoft.Maps.MapTypeId.road,
        center: new Microsoft.Maps.Location(51.19254942903923, 10.399044671663109),
        zoom: 6
      });
      var boundsBorder = new Microsoft.Maps.Polyline([
        new Microsoft.Maps.Location(55.85630905857346, 5.653838560219752),
        new Microsoft.Maps.Location(47.15630905857346, 5.653838560219752),
        new Microsoft.Maps.Location(47.15630905857346, 15.408189134456976),
        new Microsoft.Maps.Location(55.85630905857346, 15.408189134456976),
        new Microsoft.Maps.Location(55.85630905857346, 5.653838560219752)
      ], { strokeColor: 'red', strokeThickness: 5 });
      map.entities.push(boundsBorder);
      map.entities.push(pins);
      map.entities.push(cityPolygon = {});
      //FetchInfo(unis.filter(a=>true));
    }, 200);
  }
  function errorCallback(message) {
    document.getElementById('printoutPanel').innerHTML = message;
  }


}
