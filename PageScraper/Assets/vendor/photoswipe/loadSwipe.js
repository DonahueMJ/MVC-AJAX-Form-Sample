    function LoadSwipe() {

            // Init empty gallery array
            var container = [];
    var loopindex = 0;
    // Loop over gallery items and push it to the array
            $('#gallery').find('figure').each(function () {
                var $link = $(this).find('a'),
                    item = {
        src: $link.attr('href'),
    w: $link.data('width'),
    h: $link.data('height'),
    title: $link.data('caption'),
    index: loopindex
};
container.push(item);
loopindex++;
});

// Define click event on gallery item
            $('a').click(function (event) {

        // Prevent location change
        event.preventDefault();

    // Define object and gallery options
    var $pswp = $('.pswp')[0],
                    options = {
        index: parseInt($(this).parent('figure').attr('data-index'),10),
    bgOpacity: 0.85,
    showHideOpacity: true
};

// Initialize PhotoSwipe
var gallery = new PhotoSwipe($pswp, PhotoSwipeUI_Default, container, options);
gallery.init();
});

};
