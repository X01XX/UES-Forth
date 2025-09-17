
\ For use outside of the GPL 3.0 license, except for stack.fs mm_array.fs link.fs list.fs tools.fs,
\ contact the Wisconsin Alumni Research Foundation (WARF).

\ Struct IDs.
\
\ Each number is prime, 5 digits, and fits within 16 bits.
\ The digit pairs [0][1], [1][2], [2][3], and [3][4] are prime. 
\ No digit appears more than twice. (avoids 111)
\ No digit appears consecutively.   (avoids 11)
\
\ Struct ids in use.
\ link   list   region Rule   RuleStore square
\ 17137, 17971, 19317, 23131, 23173,    23197
\
\ Sample Action Session Domain Need   Changes
\ 23719, 29717, 31319,  31379, 19717, 31973
\
\ Step   Plan   Step2  Plan2
\ 37171, 37379, 41719, 41737
\
\ Struct ids not yet used:
\ 43717, 47137, 47317, 53171,
\ 53173, 53197, 53717, 53719, 53731,
\ 59797, 61379, 61717, 61979.

\ Start a clean vocabulary.
cr ." Starting vocabulary UES," cr
vocabulary UES

\ Put new words into the UES vocabulary.
UES definitions
decimal

\ Foundational.
include tools.fs
include mm_array.fs     \ includes stack.fs
include link.fs
include list.fs

\ Application.
include globals.fs
include xtindirect.fs

include value.fs
include bool.fs

include region.fs
include regionlist.fs
include region2.fs
include state.fs

include changes.fs
include changeslist.fs
include changes2.fs

include sample.fs

include rule.fs
include rulestore.fs
include square.fs

include valuelist.fs
include squarelist.fs

include group.fs
include grouplist.fs

include need.fs
include needlist.fs

include action.fs
include actionlist.fs
include actionxts.fs

include step.fs
include steplist.fs

include domain.fs
include domainlist.fs

include session.fs
include plan.fs
include planlist.fs
include input.fs

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
    cr 4 spaces ." Changes mma:      " changes-mma .mma-usage
    cr 4 spaces ." Group mma:        " group-mma .mma-usage
    cr 4 spaces ." Need mma:         " need-mma .mma-usage
    cr 4 spaces ." Step mma:         " step-mma .mma-usage
    cr 4 spaces ." Plan mma:         " plan-mma .mma-usage
    cr 4 spaces ." Action mma:       " action-mma .mma-usage
    cr 4 spaces ." Domain mma:       " domain-mma .mma-usage
    cr 4 spaces ." dstack: " .s
;

' memory-use to memory-use-xt

\ Check that no struct instances are in use, stack is clear.
: test-none-in-use
    assert-link-mma-none-in-use
    assert-list-mma-none-in-use
    assert-region-mma-none-in-use
    assert-rule-mma-none-in-use
    assert-rulestore-mma-none-in-use
    assert-square-mma-none-in-use
    assert-group-mma-none-in-use
    assert-sample-mma-none-in-use
    assert-need-mma-none-in-use
    assert-step-mma-none-in-use
    assert-plan-mma-none-in-use
    assert-action-mma-none-in-use
    assert-changes-mma-none-in-use

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
include rulestore_t.fs
include state_t.fs
include input_t.fs

cr ." main.fs"

\ Init array-stacks.
601 link-mma-init
202 list-mma-init
403 region-mma-init
304 rule-mma-init
305 rulestore-mma-init
206 square-mma-init
250 sample-mma-init
 50 changes-mma-init
100 group-mma-init
200 need-mma-init
150 step-mma-init
150 plan-mma-init
 50 action-mma-init
 25 domain-mma-init

\ Free heap memory before exiting.
: free-heap ( -- )
    cr ." Freeing heap memory"
    list-mma mma-free
    link-mma mma-free
    region-mma mma-free
    rule-mma mma-free
    rulestore-mma mma-free
    square-mma mma-free
    sample-mma mma-free
    changes-mma mma-free
    group-mma mma-free
    need-mma mma-free
    step-mma mma-free
    plan-mma mma-free
    action-mma mma-free
    domain-mma mma-free
;

: init-main ( -- )
    \ Set up session.
    session-new                                 \ sess
    dup struct-inc-use-count                    \ sess  (limited usefulness, so far, but follow convention)
    dup to current-session                      \ sess

    \ Add domain 0
    4 domain-new                                \ sess dom

    \ Add actions to domain 0
    [ ' domain-0-act-1-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-2-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-3-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-4-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0
    
    \ Add a domain
    over session-add-domain                    \ sess

    \ Add domain 1
    5 domain-new                                \ sess dom1

    \ Add actions to domain 1
    [ ' domain-1-act-1-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    [ ' domain-1-act-2-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    [ ' domain-1-act-3-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    \ Add last domain
    swap session-add-domain                     \ sess dom1
;

0 value step-num
: main ( -- )
    init-main
    0 to step-num
    true
    begin
    while
        \ Inc step num
        step-num 1+ to step-num
        
        \ Print header.
        cr ." Step: " step-num .
        space ." Current state: "
        current-session .session-current-state
        cr

        80 s" Enter command: q(uit), ... > " get-user-input
        \ cr .s cr
        depth 1 <>
        if
            ." depth not equal one? " .s
            abort
        then
    repeat
    
    \ Clean up
    memory-use
    cr cr ." Deallocating ..." cr
    current-session session-deallocate
    memory-use
    test-none-in-use
;

\ Set up a test domain and action.
\ To supply number bits, max region, ms-bit, all-bits, domain-id, action-id.
\ To run tests outside of all-tests, run this first.
: test-init

    \ Set up session.
    session-new                         \ sess
    dup struct-inc-use-count            \ sess (limited usefulness, so far, but follow convention)
    to current-session                  \

    \ Set up a source for domain-inst-id, num-bits, ms-bit, all-bits, max-region, action-id.
    4 domain-new                        \ dom

    current-session                     \ dom sess
    session-add-domain                  \ dom
;

: all-tests
    test-none-in-use

    test-init

    square-tests
    depth 0<> abort" Test a stack not empty"
    square-list-tests
    depth 0<> abort" Test b stack not empty"
    region-tests
    depth 0<> abort" Test c stack not empty"
    region-list-tests
    depth 0<> abort" Test d stack not empty"
    rule-tests
    depth 0<> abort" Test e stack not empty"
    action-tests
    depth 0<> abort" Test f stack not empty"
    rulestore-tests
    depth 0<> abort" Test g stack not empty"
    state-tests
    depth 0<> abort" Test h stack not empty"
    input-test-parse-user-input

    memory-use
    cr cr ." Deallocating ..." cr
    current-session session-deallocate

    memory-use
    test-none-in-use
;
