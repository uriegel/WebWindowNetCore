namespace WebWindowNetCore.Linux;

static class ScriptInjection
{
    public static string Get() =>
$@"
const showDevTools = () => fetch('req://showDevTools')    
";
}


