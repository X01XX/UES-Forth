
\ For use outside of the GPL 3.0 license, except for stack.fs mm_array.fs link.fs list.fs,
\ contact the Wisconsin Alumni Research Foundation (WARF).

\ Struct IDs.
\
\ Each number is prime, 5 digits, and fits within 16 bits.
\ The digit pairs [0][1], [1][2], [2][3], and [3][4] are prime. 
\ No digit appears more than twice. (avoids 111)
\ No digit appears consecutively.   (avoids 11)
\
\ Struct ids in use.
\ link   list   region Rule  RuleStore square
\ 17137, 17971, 19317, 23131 23173     23197
\
\ Sample Action Session Domain
\ 23719, 29717  31319   31379

\ Struct ids not yet used:
\ 19717, 31973, 37171, 37379, 41719,
\ 41737, 43717, 47137, 47317, 53171,
\ 53173, 53197, 53717, 53719, 53731,
\ 59797, 61379, 61717, 61979.

\ Foundational.
include tools.fs
include mm_array.fs     \ includes stack.fs
include link.fs
include list.fs


\ Application.
include xtindirect.fs

include value.fs
include bool.fs

include state.fs

include sample.fs
include region.fs
include regionlist.fs

include rule.fs
include rulestore.fs

include square.fs
include squarelist.fs

include action.fs
include actionlist.fs

include domain.fs
include domainlist.fs

include session.fs

cs

: memory-use
    cr ." Memory use:"
    cr 4 spaces ." Link mma:         " link-mma .mma-usage
    cr 4 spaces ." List mma:         " list-mma .mma-usage
    cr 4 spaces ." Region mma:       " region-mma .mma-usage
    cr 4 spaces ." Rule mma:         " rule-mma .mma-usage
    cr 4 spaces ." RuleStore mma:    " rulestore-mma .mma-usage
    cr 4 spaces ." Square mma:       " square-mma .mma-usage
    cr 4 spaces ." Sample mma:       " sample-mma .mma-usage
    cr 4 spaces ." Action mma:       " action-mma .mma-usage
    cr 4 spaces ." Session mma:      " session-mma .mma-usage
    cr 4 spaces ." Domain mma:       " domain-mma .mma-usage
    cr 4 spaces ." dstack: " .s
;

\ Check that no struct instances are in use, stack is clear.
: test-none-in-use
    assert-link-mma-none-in-use
    assert-list-mma-none-in-use
    assert-region-mma-none-in-use
    assert-rule-mma-none-in-use
    assert-rulestore-mma-none-in-use
    assert-square-mma-none-in-use
    assert-sample-mma-none-in-use
    assert-action-mma-none-in-use

    depth 0<> 
    if  
        cr ." stack not empty " .s cr
    then
;

include square_t.fs
include squarelist_t.fs
include region_t.fs
include regionlist_t.fs
include rule_t.fs
include action_t.fs

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

cr ." main.fs"

\ Init array-stacks.
101 link-mma-init
102 list-mma-init
103 region-mma-init
104 rule-mma-init
105 rulestore-mma-init
106 square-mma-init
 20 sample-mma-init
 20 action-mma-init
  1 session-mma-init
  5 domain-mma-init


\ tests
\ test-state-not-a-or-not-b
\ test-rulestore
\ test-square

\ cr memory-use cr

\ cr memory-use cr

\ Free heap memory before exiting.
\ cr ." Freeing heap memory"
\ list-mma mma-free
\ link-mma mma-free
\ region-mma mma-free
\ rule-mma mma-free
\ rulestore-mma mma-free
\ square-mma mma-free

: all-tests
    square-tests
    square-list-tests
    region-tests
    region-list-tests
    rule-tests
    action-tests
;
