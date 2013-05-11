class GrowlNotifier
  def self.growl_path
    @@growl_path
  end
  
  def self.growl_path= value
    @@growl_path = value
  end
 
  def execute title, text, color
    return unless GrowlNotifier.growl_path
 
    text.gsub!('"', "''")
 
    text = text + "\n\n---"
 
    opts = ["\"#{GrowlNotifier.growl_path}\"", "\"#{text}\"", "/t:\"#{title}\""]
 
    opts << "/i:\"#{File.expand_path("#{color}.png")}\"" 
 
    `#{opts.join ' '}`
  end
end
 
class CommandShell
  def execute cmd
    puts cmd + "\n\n"
 
    str=""
    STDOUT.sync = true # That's all it takes...
    IO.popen(cmd+" 2>&1") do |pipe| # Redirection is performed using operators
      pipe.sync = true
      while s = pipe.gets
        str+=s # This is synchronous!
      end
    end
    
    puts str + "\n\n"
 
    str
  end
end
 
@growl = GrowlNotifier.new
@sh = CommandShell.new
 
GrowlNotifier.growl_path = 
  'C:\program files (x86)\Growl for Windows\growlnotify.exe'
 
def build_succeeded? results
  return !(/: error/.match results)
end
 
def file_changed full_path
  if full_path =~ /.*.\.cs$/
    puts full_path
    results = @sh.execute "rake"
 
    if(!build_succeeded? results)
      errors = results.each_line.find { |l| l =~ /: error/ }
      puts errors
 
      @growl.execute "sad panda", errors, "red"  
    else
      results = @sh.execute "rake tests"

      if(results =~ /FAILURES/)
        @growl.execute "failed", results.split("**** FAILURES ****").last, "red"
      else
        @growl.execute "passed", "all tests passed", "green"
      end
    end
  end
end
 
def tutorial
  puts "in dotnet.watchr.rb, update the file_changed method to execute rake tasks"
end
 
method_to_run = ARGV[0] #get the first argument from the command line and act accordingly
 
case method_to_run
when "tutorial" 
  tutorial
when "file_changed"
  puts ARGV
  file_changed ARGV[1].gsub("\\", "\/")[1..-1] #run the file_changed routine giving it a shell compatible file name
else
  puts "I dont know how to run: " + method_to_run
end
