#!/usr/bin/bash
ls -dtr /media/earl/UDISK/fs1* | xargs -n 1 basename

/bin/echo "Enter fs1 or fs1_YYYY_.."
read FSX
/bin/echo "< previous, > current" > outcomp.txt
for f in ~/fs1/*.fs
do
	echo $f >> outcomp.txt
	diff /media/$USER/UDISK/$FSX/$(basename -- $f) $f >> outcomp.txt
done
cat outcomp.txt
