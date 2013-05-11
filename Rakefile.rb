@msbuild_path = "#{ENV['SystemRoot']}\\Microsoft.NET\\Framework\\v4.0.30319\\msbuild.exe"
 
def build_command(sln)
  return "\"#{@msbuild_path}\" \"#{sln}\" /verbosity:quiet /nologo"
end
 
task :default do
  sh build_command("MigraDoc.Extensions.sln")
end

task :tests do
  sh ".\\packages\\nspec.0.9.66\\tools\\NSpecRunner.exe .\\src\\specs\\MigraDoc.Extensions.Html.Specs\\bin\\Debug\\MigraDoc.Extensions.Html.Specs.dll"
end
