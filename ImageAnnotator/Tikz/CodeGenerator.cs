using System.Text;

namespace ImageAnnotator.Tikz;

/// <summary>
/// Generates tikz code
/// </summary>
public class CodeGenerator {
    public required string ImagePath { get; set; }

    public string GenerateCode() {
        StringBuilder builder = new();
        _ = builder.AppendLine("\\begin{tikzpicture}");
        _ = builder.AppendLine("  \\begin{scope}");
        _ = builder.Append("    \\node[anchor=south west,inner sep=0] (image) at (0,0) { \\includegraphics[width=\\textwidth]{").Append(ImagePath).AppendLine("}}");
        _ = builder.AppendLine("  \\end{scope}");
        _ = builder.AppendLine("\\end{tikzpicture}%");

        return builder.ToString();
    }
}