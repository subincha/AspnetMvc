$(function() { 
    $("#addDept").click(function () {
        $("#addDept").hide();
        $("#addDeptForm").show();
    });
});


$(function () {
    $("#addDeptButton").click(function () {
        //alert($("#deptName").val());
        $("#addDeptForm").hide();
        $("#loadGifDept").show();
        $.post("Employee/AddDepartment"),
            {
                deptName: $("#deptName").val()
            },
            function (result) {
                $("#loadGifDept").hide();
                $("#addDeptSuccess").html(result.view);
                if (result.id != 0) {
                    $("#DepartmentId").append("<option value=" + result.id + ">" + result.item + "</option>");
                }
                $("#addDept").show();
            });
    });
});

$("#addWork").click(function () {
    $("#addWork").hide();
    $("#addWorkForm").show();
});

$(function () {
    $("#addWorkButton").click(function () {
        //alert($("#deptName").val());
        $("#addWorkForm").hide();
        $("#loadGifWork").show();
        $.post('@Url.Action("AddExpectedWorkSchedual")',
            {
                workDays: $("#workDays").val(),
                workHours: $("#workHours").val()
            },
            function (result) {
                $("#loadGifWork").hide();
                $("#addWorkSuccess").html(result.view);
                if (result.id != 0) {
                    $("#WorkSchedualId").append("<option value=" + result.id + ">" + result.item + "</option>");
                }
                $("#addWork").show();
            });
    });
});