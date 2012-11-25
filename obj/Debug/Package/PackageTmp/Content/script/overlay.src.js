function getDomain()
{
    var arr = location.href.split('/');
    return arr[0] + '//' + arr[2];
}

function center(id, callback)
{
    var _top = (($(window).height() - $('#' + id).outerHeight()) / 2) + $(window).scrollTop();
    _top = ((_top < 0) ? 30 : _top);
    var t = _top + 'px';
    var l = (($(window).width() - $('#' + id).outerWidth()) / 2) + $(window).scrollLeft() + 'px';

    $('#' + id).css({ 'top': t, 'left': l });

    callback();
}

function getCenter(id)
{

    var _top = (($(window).height() - $('#' + id).outerHeight()) / 2) + $(window).scrollTop();
    _top = ((_top < 0) ? 30 : _top);
    var t = _top + 'px';
    var l = (($(window).width() - $('#' + id).outerWidth()) / 2) + $(window).scrollLeft() + 'px';

    return [t, l];
}

function getLeftX(option)
{
    var left = 0;
    switch (option)
    {
        case 'Image':
            left = $('#overlay').outerWidth() - ($('#overlay div.x').width() - 5);
            break;
        case 'Video':
            left = $('#overlay').outerWidth() - ($('#overlay div.x').width() - 5);
            break;
        case 'PDF':
            left = $('#overlay').outerWidth() - ($('#overlay div.x').width() - 7);
            break;
        case 'Link':
            left = $('#overlay').outerWidth() - ($('#overlay div.x').width() - 5);
            break;
    }

    return left;

}

function calculateX(type, callback)
{
    switch (type)
    {
        case 'Image':
            $('#overlay div.x').css({ 'top': '-50px' }).hide();
            $('#overlay div.x').css({ 'left': getLeftX('Image') + 'px' });
            break;
        case 'Video':
            $('#overlay div.x').css({ 'top': '-53px' }).hide();
            $('#overlay div.x').css({ 'left': getLeftX('Video') + 'px' });
            break;
        case 'PDF':
            $('#overlay div.x').css({ 'top': '-31px' }).hide();
            $('#overlay div.x').css({ 'left': getLeftX('PDF') + 'px' });
            break;
        case 'Link':
            $('#overlay div.x').css({ 'top': '-25px' }).hide();
            $('#overlay div.x').css({ 'left': getLeftX('Link') + 'px' });
            break;
    }

    $('#overlay div.x').fadeTo(10, 1, function ()
    {
        $(this).show('fast',function ()
        {            
            callback();
        });
    });
}

function overlay(bOpen, type, callback)
{
    if (bOpen)
    {
        $('#gradient').fadeTo(250, 0.65, function ()
        {
            document.getElementById('gradient').style.filter = 'progid:DXImageTransform.Microsoft.Alpha(Opacity=65)';

            $('#overlay').fadeTo(10, 0.001, function ()
            {
                if ($('#data').html().length > 0)
                {
                    center('overlay', function ()
                    {
                        $('#overlay').fadeTo(250, 1, function ()
                        {
                            calculateX(type, function ()
                            {
                                $('#overlay').show(0, function ()
                                {
                                    $('#gradient').show(0, function ()
                                    {
                                        if (callback)
                                        {
                                            callback();
                                        }
                                    });
                                });
                            });
                        })

                    });
                }
                else
                {
                    center('overlay', function ()
                    {
                        $('#overlay').fadeTo(250, 1, function ()
                        {
                            calculateX(type, function ()
                            {
                                $('#overlay').show(0, function ()
                                {
                                    $('#gradient').show(0, function ()
                                    {
                                        if (callback)
                                        {
                                            callback();
                                        }
                                    });
                                });
                            });
                        })

                    });
                }
            });
        });   
    }
    else
    {
        $('#data').html('');
        try
        {
            document.getElementById('videoOverlay').stop();
        } 
        catch (e)
        {
        }

        $('#overlay').fadeTo(250, 0, function ()
        {
            $('#overlay').hide(0, function ()
            {
                $('#gradient').fadeTo(250, 0, function ()
                {
                    $('#gradient').hide(0, function ()
                    {
                        $('#overlay div.x').fadeTo(10, 0, function ()
                        {
                            $('#overlay div.x').hide(0, function ()
                            {
                                if (callback)
                                {
                                    callback();
                                }
                            });
                        });
                    });

                });
            });

        });
    }
}
function sidebarAction(type, source, isOverlay, callback)
{
    var data = $('#overlay .container #data');
    data.html('');
    switch (type)
    {
        case "Image":
            data.html('<img src="' + source + '" alt="" />');
            if (isOverlay == 1)
            {
                callOverlay(type, function ()
                {
                    if (callback)
                    {
                        callback();
                    }
                });
            }
            break;
        case "Video":
            data.html('<div id="flash"></div>');

            var flashvars = { path: '' };
            var params = { wmode: "transparent" };
            var attributes = { id: "videoOverlay" };

            //swfobject.embedSWF('@Url.Content("/Content/flash/vid.swf")', "flash", "854", "480", "10.0.0", '@Url.Content("/Content/flash/expressInstall.swf")', flashvars, params, attributes);
            var o = '<object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" width="854" height="480" align="middle" id="videoOverlay">'
            + '<param name="movie" value="/Content/flash/vid.swf?path=' + source + '"/>' + '<param name="wmode" value="transparent" />'
            + '<!--[if !IE]>-->' + '<object type="application/x-shockwave-flash" data="/Content/flash/vid.swf?path=' + source + '" width="854" height="480">'
            + '<param name="movie" value="/Content/flash/vid.swf"/>'
            + '<param name="wmode" value="transparent" />'
            + '<!--<![endif]-->'
            + '<a href="http://www.adobe.com/go/getflash">'
            + '<img src="http://www.adobe.com/images/shared/download_buttons/get_flash_player.gif" alt="Get Adobe Flash player"/>'
            + '</a>'
            + '<!--[if !IE]>-->'
            + '</object>'
            + '<!--<![endif]-->'
            + '</object>';
            $('#flash').html(o);
            if (isOverlay == 1)
            {
                callOverlay(type, function ()
                {
                    if (callback)
                    {
                        callback();
                    }
                });
            }
            break;
        case "PDF": 
            callPDFIFrame(data, getDomain() + source, function ()
            {               
                callOverlay(type, function ()
                {
                    if (callback)
                    {
                        callback();
                    }
                });
            });
            break;
        case "Link":
            if (isOverlay == 1)
            {               
                //iframe
                callIFrame(data, source, function ()
                {
                    setIFrameSize(function ()
                    {
                        callOverlay(type, function ()
                        {
                            if (callback)
                            {
                                callback();
                            }
                        });
                    });


                });
            }
            else
            {
                if (source.substr(0, 4) == 'http')
                {
                    isOverlay = 2;
                }
                if (isOverlay == 2)
                {
                    //popup
                    window.open(source, '');
                }
                else
                {
                    //internal link
                    location.href = source;
                }
            }
            break;
    }




}

function callOverlay(type, callback)
{
    overlay(true, type, function ()
    {
        if (callback)
        {
            callback();
        }
    });    
}

function setIFrameSize(callback)
{
    var style = $('#data iframe:first').contents().find('head').children('style').html();
    var w = style.split(';')[0].split(':')[1];
    var h = style.split(';')[1].split(':')[1];

    w = parseInt(w.substr(0, w.length - 2)) + 3;
    h = parseInt(h.substr(0, h.length - 2)) + 2;
    $('#data iframe:first').attr('width', w + 'px;').attr('height', h + 'px;');

    callback();
}

function callIFrame(data, source, callback)
{
    data.append('<iframe class="linkIFrame" src=""></iframe>');
    data.children('.linkIFrame').attr('src', source);
    
    data.children('.linkIFrame').load(function ()
    {        
        callback(this);
    });
}

function callPDFIFrame(data, source, callback)
{
    data.html('<iframe class="pdf" src=""></iframe>');
    data.children('.pdf').attr('src', source + "#view=FitV&scrollbar=0&navpanes=0&toolbar=0");

    callback(this);
}


