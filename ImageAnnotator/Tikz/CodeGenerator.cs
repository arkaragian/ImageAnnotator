using ImageAnnotator.Model;
using System.Collections.Generic;
using System.Text;

namespace ImageAnnotator.Tikz;

/// <summary>
/// Generates tikz code
/// </summary>
public class CodeGenerator {
    public required string ImagePath { get; set; }
    public required List<IAnnotation> Annotations { get; set; }

    public string GenerateCode() {
        StringBuilder builder = new();
        _ = builder.Append("\\begin{tikzpicture}").AppendLine();
        _ = builder.Append("  \\node[anchor=south west,inner sep=0] (image) at (0,0) { \\includegraphics[width=\\textwidth]{").Append(ImagePath).AppendLine("}};");
        if (Annotations.Count > 0) {
            _ = builder.Append("  \\begin{scope}[x={(image.south east)},y={(image.north west)}]").AppendLine();
            foreach (IAnnotation annotation in Annotations) {
                _ = builder.Append(annotation.ToCode(identation: 4)).AppendLine();
            }
            _ = builder.Append("  \\end{scope}").AppendLine();
        }
        _ = builder.Append("\\end{tikzpicture}%").AppendLine();

        return builder.ToString();
    }
}