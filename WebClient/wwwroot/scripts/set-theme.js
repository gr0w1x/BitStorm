function setTheme(theme) {
    document.body.setAttribute("data-theme", theme);
}

{
    const theme = localStorage.getItem("theme");
    if (theme !== null)
    {
        setTheme(theme);
    }
}
