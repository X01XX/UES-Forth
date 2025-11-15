#!/usr/bin/bash

# Given a directory of source files,

# Get a list of function names.

# Search the files for each function name.

# For function names that have only one reference, that is the function definition,
# and will be displayed.

srcdir=.

# Set work directory, should not exist.
wrkdir=work

# Clear/create work directory.
if [ -d $wrkdir ]
then
	rm -R $wrkdir
fi
mkdir $wrkdir

# Put each file into work directory.
/usr/bin/cat $srcdir/*.fs > $wrkdir/source.fs
if [ -e $wrkdir/unused.txt ]
then
   /usr/bin/rm $wrkdir/unused.txt
fi

# Parse out function names.
functions=`/usr/bin/sed -e 's/\\.*//g' $wrkdir/source.fs | egrep '^\s*:\s+' | /usr/bin/sed -e 's/\s*\:\s*//g' | /usr/bin/sed -e 's/\s*(.*//' > $wrkdir/functions.txt`

# Preamble.
echo "Unused functions:"
echo
functions=`/usr/bin/cat $wrkdir/functions.txt`

# For each function name.
# Check the number of references for each function name.
for line in $functions
do
 	count=`/usr/bin/grep -c -- $line $wrkdir/source.fs`
 	if [ $count -lt 2 ]
   	then
        echo "    " $line
        echo $line >> $wrkdir/unused.txt
 	fi
done
# End
echo
echo -n "Number unused functions: "
/usr/bin/cat $wrkdir/unused.txt | wc -l 
echo -n "Total functions: "
/usr/bin/cat $wrkdir/functions.txt | wc -l 

# Clean up.
rm -R $wrkdir

