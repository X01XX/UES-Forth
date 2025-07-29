
\ Constants.
\ 4  constant num-bits
\ 15 constant all-bits
\ 8  constant ms-bit

\ Init value names.
0  value mask-mma

\ Struct IDs.
\
\ Each number is prime, 5 digits, and fits within 16 bits.
\ The digit pairs [0][1], [1][2], [2][3], and [3][4] are prime. 
\ No digit appears more than twice. (avoids 111)
\ No digit appears consecutively.   (avoids 11)
\
\ Struct ids in use.
\ link   list   region struct-xts Rule  RuleStore square
\ 17137, 17971, 19317, 19717,     23131 23173     23197
\
\ Struct ids not yet used:
\ 23719, 29717, 31319,
\ 31379, 31973, 37171, 37379, 41719,
\ 41737, 43717, 47137, 47317, 53171,
\ 53173, 53197, 53717, 53719, 53731,
\ 59797, 61379, 61717, 61979.

\ Foundational.
include tools.fs
include mm_array.fs
include link.fs
include list.fs

\ Application.
include domain.fs
include region.fs
include regionlist.fs
include rule.fs
include state.fs
include rulestore.fs
include square.fs
cs

: memory-use ( -- )
    cr ." Memory use:"
    cr 4 spaces ." Link mma:         " link-mma .mma-usage
    cr 4 spaces ." List mma:         " list-mma .mma-usage
    cr 4 spaces ." Region mma:       " region-mma .mma-usage
    cr 4 spaces ." Rule mma:         " rule-mma .mma-usage
    cr 4 spaces ." RuleStore mma:    " rulestore-mma .mma-usage
    cr 4 spaces ." Square mma:       " square-mma .mma-usage
    cr 4 spaces ." dstack: " .s
;

: test-state-not-a-or-not-b
    cr
    4 5 state-not-a-or-not-b    \ list
    cr ." ~4 + ~5: " dup .region-list cr

    3 6 state-not-a-or-not-b    \ list45 list36
    cr ." ~3 + ~6: " dup .region-list cr

    2dup region-list-region-intersections   \ list45 list36 ints
    dup cr ." Possible regions = (~4 + ~5) & (~3 + ~6) = " .region-list
    cr
    memory-use
    cr
    
    \ Deallocate remaining struct instances.
    cr ." Deallocating ..."
    region-list-deallocate
    region-list-deallocate
    region-list-deallocate
    cr memory-use
;

: test-rulestore
    4 5 rule-new        \ rul1  5->4
    3 5 rule-new        \ rul2  5->3

    rulestore-new-2     \ rs2
    cr cr ." rulestore: " dup .rulestore

    cr memory-use cr

    cr ." Deallocating ..."
    rulestore-deallocate

    cr memory-use cr
;

: test-square
    cr
    4 5 square-new
    cr ." square: " dup .square  ."  stack " .s cr

    5 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

    3 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

    2 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

    3 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

    2 over square-add-result        \ sqr0 flag
    over
    cr ." square: " .square ." cng " .bool ."  stack " .s cr

\    5 over square-add-result        \ sqr0 flag
\    over
\    cr ." square: " .square ." cng " .bool ."  stack " .s cr

    cr memory-use cr
    cr ." Deallocating ..."
    square-deallocate

    cr memory-use
;

cr ." main.fs"

\ Init array-stacks.
101 link-mma-init
102 list-mma-init
103 region-mma-init
104 rule-mma-init
105 rulestore-mma-init
106 square-mma-init

\ tests
\ test-state-not-a-or-not-b
\ test-rulestore
test-square

\ Free heap memory before exiting.
\ cr ." Freeing heap memory"
\ list-mma mma-free
\ link-mma mma-free
\ region-mma mma-free
\ rule-mma mma-free
\ rulestore-mma mma-free
\ square-mma mma-free


