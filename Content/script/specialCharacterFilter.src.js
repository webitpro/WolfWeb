function cleanUp(id)
{
    var chars = ["@", "#", "$", "^", "*", "=", "{", "[", "}", "]", "\\", ";", '"', "<", ">"];
    var s = $('#' + id).val();
    var v = "";
    for (i = 0; i < s.length; i++)
    {
        if (chars.indexOf(s.charAt(i)) != -1)
        {
            //remove character
            v += '-';
        }
        else
        {
            //reassign character to new value
            v += s.charAt(i);
        }
    }

    return v;
}

function urlSafe(id)
{
    var s = $('#' + id).val();
    var v = "";
    
    var arr = s.split(' ');
    for (var i = 0; i < arr.length; i++)
    {
        v += "_" + urlFilter(arr[i].toLowerCase());
    }

    return v.substr(1);
}

function urlFilter(s)
{
    var v = "";
    var chars = ["~", "`", "@", "#", "$", "%", "^", "&", "*", "(", ")", "+", "=", "{", "[", "}", "]", "\\", "|", ";", ":", '"', "'", "<", ">", ".", ",", "?", "/"];
    for (i = 0; i < s.length; i++)
    {
        if (chars.indexOf(s.charAt(i)) != -1)
        {
            //remove character
            v += '';
        }
        else
        {
            //reassign character to new value
            v += s.charAt(i);
        }
    }

    return v;
}