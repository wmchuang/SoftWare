layui.use(['form', 'layer', 'table', 'laytpl'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : top.layer,
        $ = layui.jquery,
        laytpl = layui.laytpl,
        layedit = layui.layedit,
        table = layui.table;

    var tableIns = table.render({
        elem: '#list',
        url: '/nlog/LoadData/',
        cellMinWidth: 95,
        page: true,
        height: "full-125",
        limits: [10, 15, 20, 25],
        limit: 10,
        id: "listTable",
        cols: [[
            { type: "checkbox", fixed: "left", width: 50 },
            { field: "id", title: 'Id', width: 80, align: "center" },
            { field: 'url', title: '地址', width: 320, align: "left" },
            { field: 'parameter', title: '参数', width: 300, align: "left" },
            { field: 'message', title: '错误信息', minWidth: 480, align: "left" },
            { field: 'date', title: '创建时间', width: 220, align: 'center' },
            { title: '操作', width: 80, templet: '#listBar', fixed: "right", align: "center" }
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

    //批量删除
    $(".delAll_btn").click(function () {
        var checkStatus = table.checkStatus('listTable'),
            data = checkStatus.data,
            id = [];
        if (data.length > 0) {
            for (var i in data) {
                id.push(data[i].Id);
            }
            layer.confirm('确定删除选中的信息？', { icon: 3, title: '提示信息' }, function (index) {
                //获取防伪标记
                del(id,0);
            });
        } else {
            layer.msg("请选择需要删除的信息");
        }
    });


    //列表操作
    table.on('tool(list)', function (obj) {
        var layEvent = obj.event,
            data = obj.data;

        if (layEvent === 'edit') { //编辑
            add(data);
        } else if (layEvent === 'del') { //删除
            layer.confirm('确定删除此信息？', { icon: 3, title: '提示信息' }, function (index) {
                del(data.id,index);
            });
        }
    });

    function del(Id, index) {
        $.ajax({
            type: 'POST',
            url: '/nlog/Delete/',
            data: { Id: Id },
            dataType: "json",
            success: function (data) {//res为相应体,function为回调函数
                layer.msg(data.message, {
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
