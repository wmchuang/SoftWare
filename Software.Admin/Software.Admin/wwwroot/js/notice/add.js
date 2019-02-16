layui.use(['form', 'layer', 'layedit', 'laydate', 'upload'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : top.layer,
        layedit = layui.layedit,
        $ = layui.jquery;
    //用于同步编辑器内容到textarea
    layedit.sync(editIndex);

    form.on("submit(addNotice)", function (data) {
        $.ajax({
            type: 'POST',
            url: '/notice/AddOrModify/',
            data: {
                Id: $("#Id").val(),  //主键
                title: $(".title").val(),  //文章标题
                content: layedit.getContent(editIndex).split('<audio controls="controls" style="display: none;"></audio>')[0],
            },
            dataType: "json",
            success: function (res) {//res为相应体,function为回调函数
                if (res.status) {
                    var alertIndex = layer.alert(res.message, { icon: 1 }, function () {
                        layer.closeAll("iframe");
                        //刷新父页面
                        parent.location.reload();
                        top.layer.close(alertIndex);
                    });
                    //$("#res").click();//调用重置按钮将表单数据清空
                }
                else {
                    layer.alert(res.message, { icon: 5 }, function (index) {
                        //关闭弹窗
                        layer.close(index);
                        $("#res").click();//调用重置按钮将表单数据清空
                    });

                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                layer.alert('操作失败！！！' + XMLHttpRequest.status + "|" + XMLHttpRequest.readyState + "|" + textStatus, { icon: 5 });
            }
        });
        return false;
    });
    form.verify({
        title: function (val) {
            if (val == '') {
                return "文章标题不能为空";
            }
        }
    }); 

    var editIndex = layedit.build('news_content', {
        height: 535,
        uploadImage: {
            url: "/notice/UploadImage"
        }
    });
});