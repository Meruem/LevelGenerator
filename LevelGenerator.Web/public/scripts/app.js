var repaint = response => {
  var canvas = document.getElementById('generateResult');
  canvas.width = response.length * 10;
  canvas.height = response[0].length * 10;
  var ctx = canvas.getContext('2d');
  for(var i = 0; i < response.length; i++) {
    for(var j = 0; j < response[i].length; j++) {
      switch(response[i][j]) {
        case 0:
          ctx.fillStyle = 'rgb(255,0,0)';
          break;
        case 1:
          ctx.fillStyle = 'rgb(0,255,0)';
          break;
        case 2:
          ctx.fillStyle = 'rgb(0,0,255)';
          break;
        default:
          ctx.fillStyle = 'rgb(255, 255, 0)';
      }
      ctx.fillRect(i*10, j*10,10,10);
    }
  }
}

new Vue({
  el: '#app',
  data: {
    showGenerate : true,
    showDesigner : false,
    width : '30',
    height : '30',
    response : []
  },
  methods: {
    changeTab: function(nr) {
      if (nr == 1) {
        this.showGenerate = true;
        this.showDesigner = false;
      } else if (nr == 2) {
        this.showGenerate = false;
        this.showDesigner = true;
      }
    },
    generate: function () {
      fetch('./generate/' + this.width + '/' + this.height)
        .then(res => {
          return res.json();
        }).then(json => {
          this.response = json;
          repaint(this.response);
        })
    }
  }
})