

namespace sim;

public class Gui : GuiBase
{
    public Label label{get;} 
    public Label label1 { get; }
    public Gui(Comm comm) : base(comm)
    {
        label = new Label("label", comm);
        label1 = new Label("Program_LabelToolNrSpindleDesc", comm);
    }
}
