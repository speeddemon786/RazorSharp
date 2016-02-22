var pageName = window.location.pathname.substring(window.location.pathname.lastIndexOf("/") + 1, window.location.pathname.lenght).toLowerCase();
if (pageName.match("^editpage") || pageName.match("^newpage") || pageName.match("^widgetsonpage"))
{
    $('#allpages').toggleClass('active');
}
else if (pageName.match("^editwidget") || pageName.match("^newwidget"))
{
    $('#allwidgets').toggleClass('active');
}
else if (pageName.match("^newmenu"))
{
    $('#allmenu').toggleClass('active');
}
else if (pageName.match("^newaddon"))
{
    $('#alladdons').toggleClass('active');
}
else if (pageName.match("^edituser") || pageName.match("^newuser"))
{
    $('#allusers').toggleClass('active');
}
else if (pageName === "")
{
    $('#default').toggleClass('active');
}
$('#' + pageName).toggleClass('active');