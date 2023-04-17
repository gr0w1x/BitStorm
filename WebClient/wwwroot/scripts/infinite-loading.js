function initializeInfiniteScrolling(
    lastIndicator,
    componentInstance,
    onLoading = "OnLoading",
    rootQuery = undefined,
) {
    const options = {
        root: rootQuery !== undefined ? document.querySelector(rootQuery) : findClosestScrollContainer(),
        rootMargin: "0px",
        threshold: 0,
    };

    const observer = new IntersectionObserver(async (entries) => {
        for (const entry of entries) {
            if (entry.isIntersecting) {
                observer.unobserve(lastIndicator);
                await componentInstance.invokeMethodAsync(onLoading);
            }
        }
    }, options);

    observer.observe(lastIndicator);

    return {
        dispose: () => dispose(observer),
        onNewItems: () => {
            observer.unobserve(lastIndicator);
            observer.observe(lastIndicator);
        },
    };
}

function dispose(observer) {
    observer.disconnect();
}

function findClosestScrollContainer(element) {
    while (element) {
        const style = getComputedStyle(element);
        if (style.overflowY.includes("scroll", "auto")) {
            return element;
        }
        element = element.parentElement;
    }
    return null;
}
