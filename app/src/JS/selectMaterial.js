(() => {
    $.fn.selectMaterial = function () {
        this.each((i, item) => {
            var select1 = $(item);
            select1.addClass("initialized");
            var pin=$(`<span class="caret">▼</span>`)
                .insertBefore(select1);
            var input1 = $(`<input  type="text" class="form-control" 
                        readonly="true" value="" role="listbox" aria-multiselectable="false" aria-disabled="false"
                        aria-required="false" aria-haspopup="true" aria-expanded="false">`)
                .insertBefore(select1);
            let ul1 = $(`<ul  
        class="dropdown-content select-dropdown w-100" 
        style="display: none; width: 429.2px; position: absolute; top: 0px; left: 0px; opacity: 1;""> `)
                .insertBefore(select1);
            var lis = [];
            var options = $('option', select1).each((i, e) => {
                e = $(e)
                lis.push($(`<li role="option" value="${e.val()}"><span class="filtrable ">${e.text()}</span></li>`)
                    .appendTo(ul1)
                    .addClass(
                        (e[0].selected ? "active selected" : "")
                        + " " +
                        (e[0].disabled ? "disabled" : "")
                    )[0]);
                if (e[0].selected && e.val())
                    input1.val(e.text());
            });
            lis = $(lis);
            input1.focus();
            setTimeout(input1.blur.bind(input1), 100);
            input1.focus((e) => {
                ul1.show();

            });
            input1.click((e) => {
                ul1.show();

            });
            pin.click((e) => {
                input1.focus();

            });
            var intervalBlur=null;
            lis.click(e => {
                e = $(e.target);
                input1.focus();
                if (e.hasClass("disabled")) {
                    clearInterval(intervalBlur);
                    intervalBlur=null;
                    return;
                }
                lis.removeClass("selected");
                lis.removeClass("active");
                e.parent().addClass("selected");
                e.parent().addClass("active");
                input1.val(e.text());
                select1.val(e.parent().val());
            });
            input1.blur((e) => {
                intervalBlur=setTimeout(ul1.hide.bind(ul1), 200);
            });
            select1.change(e=>{
                lis.each((i,a)=>{
                    i=$(a);
                    if(i.val()==select1.val())
                    {
                        lis.removeClass("selected");
                        lis.removeClass("active");
                        i.addClass("selected");
                        i.addClass("active");
                        input1.val(i.text());
                    }
                })
            });
        });
    };

})();