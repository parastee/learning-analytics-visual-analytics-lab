Array.prototype.sum = function (func) {
  if (!func) func = (a) => a;
  var sumValue = 0;
  this.forEach(a => {
    var v = Number.parseFloat(func(a) || 0);
    sumValue = sumValue + (Number.isNaN(v) ? 0 : v);
  });
  return sumValue;
}

Array.prototype.avg = function (func) {
  return this.sum(func) / this.length;
}
$(() => {
  new WOW().init();
  $("select.mdb-select.md-form").selectMaterial()
  let toggleModal = $('#toggleModal').click(() => {
    $('#form3').val(sessionStorage.getItem('name'));
    $('#form2').val(sessionStorage.getItem('email'));
    $('#casestudy').val(sessionStorage.getItem('casestudy')).change();
    $('#degree').val(sessionStorage.getItem('degree')).change();
});
  window.ids = location.search.split('ids=')[1].split('&')[0].split(',');
  window.MainselectedUni = window.selectedUni = uinpoint.filter(u => ids.filter(id => u.uni.UniId == id).length > 0).sort((a, b) => a.uni.Rank - b.uni.Rank);
  $table = $('#tbluni');

  window.selectedUni.map(u => u.uni).forEach((u, i) => {
    $('<tr><th scope="row">' + (i + 1) + '</th></tr>')
      .appendTo($table.find('tbody'))
      .append($('<td>')
        .append($('<input type="checkbox"  checked="checked" class="form-check-input" id="slct' + (i) + '" >'))
        .append($('<label class="form-check-label" for="slct' + (i) + '"></label>')))
      .append($('<td>').text(u.UniName))
      .append($('<td>').text(u.City))
      .append($('<td>').text(u.ControlType))
      .append($('<td>').text(u.StudyAbroad))
      .append($('<td>').text(u.Rank))
    $table.find('tbody')
  });




  let linchartCatName = ["teacher_support", "support_in_studies", "courses_offered", "study_organisation", "exams", "job_market_preparation", "support_for_stays_abroad", "it_infrastructure", "overall_study_situation"]
  let linchartCatTitle = ["Teacher Support", "Support in Studies", "Courses offered", "Study Organisation", "Exams", "Job Market Preparation", "Support for Stays Abroad", "IT Infrastructure", "overall_study_situation"]

  selectedUni.filter(a => !!a.dparts)
    .map(a => {
      a.other = ({
        ...a.other,
        number_of_bachelors_degree_students: a.dparts.sum(b => b.number_of_bachelors_degree_students),
        number_of_master_degree_students: a.dparts.sum(b => b.number_of_master_degree_students)
      });
      linchartCatName.forEach(prop => a.other[prop] = a.dparts.avg(b => b[prop]));
      return a;
    });


  let stackbarchart = { destroy: () => { } };
  let linechart = { destroy: () => { } };

  selectrowchange({ target: { id: 0 } });

  function selectrowchange(e) {
    var allrowcheckbox = $('#tbluni input[type="checkbox"]:not(#slctAll)');
    var slctAll = $('#slctAll');
    if (e.target.id == 'slctAll') {
      allrowcheckbox.prop('checked', slctAll.prop("checked"));
      toggleModal.prop('disabled', !slctAll.prop("checked"))
    }
    else {
      var alltrue = true;
      var allfalse = true;
      allrowcheckbox.each((i, a) => {
        if ($(a).prop("checked"))
          allfalse = false;
        else
          alltrue = false;
      });

      slctAll.prop('checked', alltrue);
      slctAll.prop('indeterminate', !alltrue && !allfalse);
      toggleModal.prop('disabled', allfalse)
    }
    selectedUni = [];
    var delselectedUni = [];
    allrowcheckbox.each((i, a) => {
      if ($(a).prop("checked")) {
        selectedUni.push(MainselectedUni[Number.parseInt(a.id.replace('slct', ""))]);
      }
      else {
        delselectedUni.push(MainselectedUni[Number.parseInt(a.id.replace('slct', ""))]);
      }
    });
    var i = 0;
    d3.select("#radarchart1>svg").remove();
    let radarData = selectedUni.map(a => a.other)
      // .map(u=>({
      //   name:u.name,
      //   medicine_health_science: Number.parseFloat( u.medicine_health_science.replace('%',"")),
      //   natural_sciences_mathematics: Number.parseFloat( u.natural_sciences_mathematics.replace('%',"")),
      //   law_economic_and_social_sciences: Number.parseFloat( u.law_economic_and_social_sciences.replace('%',"")),
      //   humanities_languages: Number.parseFloat( u.humanities_languages.replace('%',"")),
      //   engineering_science_incl_computer_science: Number.parseFloat( u.engineering_science_incl_computer_science.replace('%',"")),
      //   other_studies:Number.parseFloat(  u.other_studies.replace('%',""))
      // }))
      .map(u => ({
        name: u.name,
        medicine_health_science: Number.parseFloat(u.medicine_health_science_per.replace('%', "")) * 100,
        natural_sciences_mathematics: Number.parseFloat(u.natural_sciences_mathematics_per.replace('%', "")) * 100,
        law_economic_and_social_sciences: Number.parseFloat(u.law_economic_and_social_sciences_per.replace('%', "")) * 100,
        engineering_science_incl_computer_science: Number.parseFloat(u.engineering_science_incl_computer_science_per.replace('%', "")) * 100,
        humanities_languages: Number.parseFloat(u.humanities_languages_per.replace('%', "")) * 100,
        other_studies: Number.parseFloat(u.other_studies_per.replace('%', "")) * 100
      }))
      .map(u => ({
        className: "legen" + (++i),
        name: u.name,
        axes: [{ axis: "Medicine Health", value: u.medicine_health_science },
        { axis: "Natural Sciences & Mathematics", value: u.natural_sciences_mathematics },
        { axis: "Law Economic & Social Sciences", value: u.law_economic_and_social_sciences },
        { axis: "Humanities Languages", value: u.humanities_languages },
        { axis: "Engineering Science & Computer Science", value: u.engineering_science_incl_computer_science },
        { axis: "Other Studies", value: 100 - (u.medicine_health_science + u.natural_sciences_mathematics + u.law_economic_and_social_sciences + u.humanities_languages + u.engineering_science_incl_computer_science) }]
      }));
    if (radarData.length == 0) {
      radarData = [{
        className: "",
        name: "",
        axes: [{ axis: "Medicine Health", value: 0 },
        { axis: "Natural Sciences & Mathematics", value: 0 },
        { axis: "Law Economic & Social Sciences", value: 0 },
        { axis: "Humanities Languages", value: 0 },
        { axis: "Engineering Science & Computer Science", value: 0 },
        { axis: "Other Studies", value: 0 }]
      }];
    }
    RadarChart.defaultConfig.radius = 3;
    var chart = RadarChart.chart();
    var cfg = chart.config(); // retrieve default config
    cfg.w = 400;
    cfg.h = 400;
    $('#radarchart1').width(cfg.w + 300);
    RadarChart.draw("#radarchart1", radarData, { ...cfg, maxValue: 100, levels: 5, factor: 0.8, open: true });

    stackbarchart.destroy();
    stackbarchart = c3.generate({
      bindto: '#barchart1',
      data: {
        columns: [
          ['Bachelor'].concat(selectedUni.map(a => a.other.number_of_bachelors_degree_students)),
          ['Master'].concat(selectedUni.map(a => a.other.number_of_master_degree_students))
        ],
        type: 'bar',
        groups: [
          ['Bachelor', 'Master']
        ]
      },
      axis: {
        x: {
          type: 'category',
          categories: selectedUni.map(a => a.uni.Acronym || a.uni.UniName)
        }
      }
    });


    var cols = selectedUni.map(u => [u.uni.Acronym || u.other.UniName].concat(linchartCatName.map(prop => (Math.round((u.other[prop]*100/6)*100)/100))));
    var g={};
    selectedUni.forEach(u => g[u.uni.Acronym || u.other.UniName]='line');
    linechart.destroy();
    $('#linechart1 svg').remove();
    linechart = c3.generate({
      bindto: '#linechart1',
      data: {
        columns: [cols[0],cols[1]],
        types: g,
        groups: [
          selectedUni.map(a => a.uni.Acronym || a.other.UniName)
        ]
      },
      axis: {
        max:100,
        x: {
          type: 'category',
          categories: linchartCatTitle
        }
      }
    });
    linechart.axis.max(100);
  }
  $('#tbluni input[type=checkbox]').click(selectrowchange);
});
