lines = textBox.get_Text().split("\n")
textBox.set_Text(lines.zip((1..lines.length).to_a).map do |a| a[1].to_s + " " + a[0] end.join("\n"))
