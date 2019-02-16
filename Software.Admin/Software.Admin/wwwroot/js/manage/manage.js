layui.use(['form', 'layer', 'laydate', 'table', 'laytpl'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : top.layer,
        $ = layui.jquery,
        laydate = layui.laydate,
        laytpl = layui.laytpl,
        table = layui.table;

    form.on("submit(changePwd)", function (data) {
        //获取防伪标记
        $.ajax({
            type: 'POST',
            url: '/Manage/changePwd/',
            data: {
                oldPwd: $("#oldPwd").val(),
                newPwd: $("#newPwd").val()
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

    //添加验证规则
    form.verify({
        //oldPwd: function (value, item) {
        //    if (value != "123456") {
        //        return "密码错误，请重新输入！";
        //    }
        //},
        newPwd: function (value, item) {
            if (value.length < 6) {
                return "密码长度不能小于6位";
            }
        },
        confirmPwd: function (value, item) {
            if (!new RegExp($("#newPwd").val()).test(value)) {
                return "两次输入密码不一致，请重新输入！";
            }
        }
    });
});