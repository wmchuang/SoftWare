layui.use(['form', 'layer', 'table', 'laytpl'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : top.layer,
        $ = layui.jquery,
        laytpl = layui.laytpl,
        layedit = layui.layedit,
        table = layui.table;

    var tableIns = table.render({
        elem: '#list',
        url: '/notice/LoadData/',
        cellMinWidth: 95,
        page: true,
        height: "full-125",
        limits: [10, 15, 20, 25],
        limit: 10,
        id: "listTable",
        cols: [[
            //{ type: "checkbox", fixed: "left", width: 50 },
            { field: "Id", title: 'Id', width: 80, align: "center" },
            { field: 'Title', title: '标题', minWidth: 180, align: "center" },
            { field: 'CreateTime', title: '创建时间', align: 'center' },
            { title: '操作', minWidth: 80, templet: '#listBar', fixed: "right", align: "center" }
        ]]
    });

    $(".search_btn").on("click", function () {
        table.reload("listTable", {
            page: {
                curr: 1 //重新从第 1 页开始
            },
            where: {
                key: $(".searchVal").val()  //搜索的关键字
            }
        });
       
    });

    //添加
    function add(edit) {
        var tit = "添加通知";
        if (edit) {
            tit = "编辑通知";
        }
        var index = layui.layer.open({
            title: tit,
            type: 2,
            //anim: 1,
            //area: ['500px', '90%'],
            content: "/notice/AddOrModify/",
            success: function (layero, index) {
                var body = layui.layer.getChildFrame('body', index);
                console.info(edit);
                if (edit) {
                    body.find("#Id").val(edit.Id);
                    body.find(".title").val(edit.Title);
                    //layedit.setContent(editIndex, edit.Content);
                    body.find("#news_content").val(edit.Content);
                    form.render();
                }
            }
        });

        layui.layer.full(index);
        $(window).on("resize", function () {
            layui.layer.full(index);
        });
    }
    $(".addbtn").click(function () {
        add();
    });


    //列表操作
    table.on('tool(list)', function (obj) {
        var layEvent = obj.event,
            data = obj.data;

        if (layEvent === 'edit') { //编辑
            add(data);
        } else if (layEvent === 'del') { //删除
            layer.confirm('确定删除此通知？', { icon: 3, title: '提示信息' }, function (index) {
                del(data.Id,index);
            });
        }
    });

    function del(Id, index) {
        $.ajax({
            type: 'POST',
            url: '/notice/Delete/',
            data: { Id: Id },
            dataType: "json",
            success: function (data) {//res为相应体,function为回调函数
                layer.msg(data.ResultMsg, {
                    time: 1000 
                }, function () {
                    tableIns.reload();
                    layer.close(index);
                });
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                layer.alert('操作失败！！！' + XMLHttpRequest.status + "|" + XMLHttpRequest.readyState + "|" + textStatus, { icon: 5 });
            }
        });
    }
});
