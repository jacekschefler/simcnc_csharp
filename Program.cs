using static System.Console;
using sim;
var device = new Device(ip: "192.168.1.108", port: 60000);
//WriteLine(device.GetState());

WriteLine(device.IsGcodeFileLoaded());

//device.OpenGcodeFile("\\\\192.168.1.200\\grawerki\\testyyy\\M\\M__07 - 5.5-R2.tap");
var gui = new Gui(device.Comm);

WriteLine(gui.label.Text);


 