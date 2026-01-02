
\ For use outside of the GPL 3.0 license, except for stack.fs mm_array.fs link.fs list.fs tools.fs stackprint.fs, struct.fs, structlist.fs
\ contact the Wisconsin Alumni Research Foundation (WARF).

\ The Linux command: alias words='grep "^: "'
\ allows a quick list of functions in a file, like "words session.fs" or more, "words *list.fs"
\ Words can be filtered, like "words list.fs | grep remove" or "words list.fs | grep -- list-set-"
\ A brief comment on the same line as the function name can be helpful.
\ For a bash shell user, the command can be added at the end of ~/.bashrc to make it permanent.
 
\ Struct IDs.
\
\ Each number is prime, 5 digits, and fits within 16 bits.
\ The digit pairs [0][1], [1][2], [2][3], and [3][4] are prime.
\ No digit appears more than twice. (avoids 111)
\ No digit appears consecutively.   (avoids 11)
\
\ Struct ids in use.
\ Link   List   Region Rule   RuleStore Square
\ 17137, 17971, 19317, 23131, 23173,    23197
\
\ Sample Action Session Domain Need   Changes
\ 23719, 29717, 31319,  31379, 19717, 31973
\
\ PlanStep   Plan   Group  Rate   RegionCorrRate  RegionCorr
\ 37171,     37379, 43717, 41719, 41737,          47317
\
\ RuleCorr  Changescorr PathStep Plancorr
\ 53171     53173       53197    53717
\
\ Struct ids not yet used:
\ 53719,
\ 53731, 59797, 61379, 61717, 61979.

\ Start a clean vocabulary.
cr ." Starting vocabulary UES," cr
vocabulary UES

\ Put new words into the UES vocabulary.
UES definitions

decimal

include xtindirect.fs
include bool.fs

include tools.fs
include struct.fs

include mm_array.fs     \ includes stack.fs
include link.fs
include list.fs
include structlist.fs

\ Application.
include globals.fs

include value.fs

include region.fs
include region2.fs

include regionlist.fs

include state.fs

include changes.fs
include changeslist.fs
include changes2.fs
include changescorr.fs

include sample.fs

include rule.fs
include rulestore.fs
include rulelist.fs

include square.fs

include valuelist.fs
include squarelist.fs

include need.fs
include needlist.fs

include planstep.fs
include plansteplist.fs

include regioncorr.fs
include regioncorrlist.fs
include rate.fs
include regioncorrrate.fs
include regioncorrratelist.fs
include rulecorr.fs

include plan.fs
include planlist.fs
include plan_t.fs
include plancorr.fs
include plancorrlist.fs

include pathstep.fs
include pathsteplist.fs

include group.fs
include grouplist.fs

include action.fs
include actionlist.fs
include actionxts.fs

include domain.fs
include domainlist.fs

include session.fs

include input.fs

include stackprint.fs

cs

: memory-use
    cr ." Memory use:"
    cr #4 spaces ." Link mma:           " link-mma .mma-usage
    cr #4 spaces ." List mma:           " list-mma .mma-usage
    cr #4 spaces ." Region mma:         " region-mma .mma-usage
    cr #4 spaces ." RegionCorr mma:     " regioncorr-mma .mma-usage
    cr #4 spaces ." Rule mma:           " rule-mma .mma-usage
    cr #4 spaces ." RuleStore mma:      " rulestore-mma .mma-usage
    cr #4 spaces ." RuleCorr mma:       " rulecorr-mma .mma-usage
    cr #4 spaces ." Square mma:         " square-mma .mma-usage
    cr #4 spaces ." Sample mma:         " sample-mma .mma-usage
    cr #4 spaces ." Changes mma:        " changes-mma .mma-usage
    cr #4 spaces ." ChangesCorr mma:    " changescorr-mma .mma-usage
    cr #4 spaces ." Group mma:          " group-mma .mma-usage
    cr #4 spaces ." Need mma:           " need-mma .mma-usage
    cr #4 spaces ." PlanStep mma:       " planstep-mma .mma-usage
    cr #4 spaces ." PlanCorr mma:       " plancorr-mma .mma-usage
    cr #4 spaces ." PathStep mma:       " pathstep-mma .mma-usage
    cr #4 spaces ." Plan mma:           " plan-mma .mma-usage
    cr #4 spaces ." Rate mma:           " rate-mma .mma-usage
    cr #4 spaces ." RegionCorrRate mma: " regioncorrrate-mma .mma-usage
    cr #4 spaces ." Action mma:         " action-mma .mma-usage
    cr #4 spaces ." Domain mma:         " domain-mma .mma-usage
    cr #4 spaces ." Session mma:        " session-mma .mma-usage
    cr #4 spaces ." dstack: " .s
;

' memory-use to memory-use-xt

\ Check that no struct instances are in use, stack is clear.
: test-none-in-use
    assert-link-mma-none-in-use
    assert-list-mma-none-in-use
    assert-region-mma-none-in-use
    assert-regioncorr-mma-none-in-use
    assert-rule-mma-none-in-use
    assert-rulestore-mma-none-in-use
    assert-rulecorr-mma-none-in-use
    assert-square-mma-none-in-use
    assert-group-mma-none-in-use
    assert-sample-mma-none-in-use
    assert-need-mma-none-in-use
    assert-planstep-mma-none-in-use
    assert-pathstep-mma-none-in-use
    assert-plan-mma-none-in-use
    assert-plancorr-mma-none-in-use
    assert-action-mma-none-in-use
    assert-changes-mma-none-in-use
    assert-changescorr-mma-none-in-use
    assert-rate-mma-none-in-use
    assert-regioncorrrate-mma-none-in-use

    depth 0<>
    if
        cr ." stack not empty " .s cr
    then
;

include state_t.fs
include square_t.fs
include squarelist_t.fs
include region_t.fs
include regionlist_t.fs
include rule_t.fs
include action_t.fs
include rulestore_t.fs
include state_t.fs
include input_t.fs
include regioncorr_t.fs
include regioncorrlist_t.fs
include session_t.fs

cr ." main.fs"

\ Init array-stacks.
#3500 link-mma-init
#1902 list-mma-init
#3003 region-mma-init
#0500 regioncorr-mma-init
#0804 rule-mma-init
#0405 rulestore-mma-init
#0404 rulecorr-mma-init
#0306 square-mma-init
#0250 sample-mma-init
#0550 changes-mma-init
#0250 changescorr-mma-init
#0100 group-mma-init
#0200 need-mma-init
#0150 planstep-mma-init
#0370 pathstep-mma-init
#0150 plan-mma-init
#0040 plancorr-mma-init
#0050 action-mma-init
#0025 domain-mma-init
#0100 rate-mma-init
#0100 regioncorrrate-mma-init
#0005 session-mma-init
cr

\ Free heap memory before exiting.
: free-heap ( -- )
    cr ." Freeing heap memory"
    list-mma mma-free
    link-mma mma-free
    region-mma mma-free
    regioncorr-mma mma-free
    rule-mma mma-free
    rulestore-mma mma-free
    rulecorr-mma mma-free
    square-mma mma-free
    sample-mma mma-free
    changes-mma mma-free
    changescorr-mma mma-free
    group-mma mma-free
    need-mma mma-free
    planstep-mma mma-free
    pathstep-mma mma-free
    plan-mma mma-free
    plancorr-mma mma-free
    action-mma mma-free
    domain-mma mma-free
    rate-mma mma-free
    regioncorrrate-mma mma-free
    session-mma mma-free
;

: init-main ( -- )
    \ Set up session.
    current-session-new                         \ session instance added to session stack.
    current-session                             \ sess

    \ Add domain 0
    #4 domain-new                                \ sess dom

    \ Add actions to domain 0
    [ ' domain-0-act-1-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-2-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-3-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-4-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-5-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    [ ' domain-0-act-6-get-sample ] literal     \ sess dom0 xt
    over domain-add-action                      \ sess dom0

    \ Add a domain
    over session-add-domain                    \ sess

    \ Add domain 1
    #5 domain-new                               \ sess dom1

    \ Add actions to domain 1
    [ ' domain-1-act-1-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    [ ' domain-1-act-2-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    [ ' domain-1-act-3-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    [ ' domain-1-act-4-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    [ ' domain-1-act-5-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    [ ' domain-1-act-6-get-sample ] literal     \ sess dom1 xt
    over domain-add-action                      \ sess dom1

    \ Add last domain
    over session-add-domain                     \ sess dom1

    s" (X1X1 01X1X)" regioncorr-from-string-a   \ sess regc
    -1 #2 rate-new                              \ sess regc rt
    regioncorrrate-new                          \ sess regc-rt
    over session-add-regioncorrrate             \ sess

    s" (1XX1 01X1X)" regioncorr-from-string-a   \ sess regc
    #-2 0 rate-new                              \ sess
    regioncorrrate-new                          \ sess regc-rt
    over session-add-regioncorrrate             \ sess

    s" (00X1 11XXX)" regioncorr-from-string-a   \ sess regc
    0 #2 rate-new                               \ sess
    regioncorrrate-new                          \ sess regc-rt
    over session-add-regioncorrrate             \ sess

    .session
;

0 value step-num
: main ( -- )
    init-main
    0 to step-num
    true
    begin
    while
        \ Inc step num.

        step-num 1+ to step-num

        \ Print header.
        cr ." ***************************"
        cr ." Step: " step-num .
        space ." Current state: "
        current-session dup .session-current-states     \ sess
        space ." Reachable "
        dup .session-reachable-regions                  \ sess
        space

        dup session-get-current-rate                    \ sess rate

        space ." rate: "
        dup .rate                                       \ sess rate
        space ." Status: "
        dup rate-get-positive
        0= if
            \ No positive value.
            dup rate-get-negative
            0= if
                \ No negative value.
                ." Neutral"
            else
                 \ Some negative value.
                ." Negative"
            then
        else
            \ Some positive value.
            dup rate-get-negative
            0= if
                \ No negative value.
                ." Positive"
            else
                \ Some negative value.
                ." Conflicted"
            then
        then
        rate-deallocate                                 \ sess
        drop                                            \
        cr

        #80 s" Enter command: > " get-user-input
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
    current-session-deallocate
    memory-use
    test-none-in-use
;

\ Set up a test domain and action.
\ To supply number bits, max region, ms-bit, all-bits, domain-id, action-id.
\
\ To run tests outside of all-tests, run this first, followed by test-end
\
: test-init

    \ Set up session.
    current-session-new                 \ Session instance added to session stack.

    \ Set up a source for domain-inst-id, num-bits, ms-bit, all-bits, max-region, action-id.
    #4 domain-new                       \ dom
    current-session                     \ dom sess

    session-add-domain                  \ dom

    #5 domain-new                       \ dom
    current-session                     \ dom sess
    session-add-domain                  \ dom

    \ Set current domain to the first.
    \ Most tests assume a 4-bit domain.
    0 set-domain
;

: test-end
    memory-use
    cr cr ." Deallocating ..." cr
    current-session-deallocate

    memory-use
    session-stack
    stack-empty?
    if
        test-none-in-use
    then
;

: all-tests
    test-none-in-use

    test-init

    square-tests
    depth 0<> abort" Square stack tests not empty"

    square-list-tests
    depth 0<> abort" Square-list tests stack not empty"

    region-tests
    depth 0<> abort" Region tests stack not empty"

    region-list-tests
    depth 0<> abort" Region-list tests stack not empty"

    rule-tests
    depth 0<> abort" Rule tests stack not empty"

    action-tests
    depth 0<> abort" Action tests stack not empty"

    rulestore-tests
    depth 0<> abort" Rulestore tests stack not empty"

    state-tests
    depth 0<> abort" State tests stack not empty"

    input-tests
    depth 0<> abort" Input tests stack not empty"

    \ Tests that assume a 4-bit domain-0 and a 5-bit domain-1 should be last,
    \ as they may change the current domain.
    regioncorr-tests
    depth 0<> abort" regioncorr tests stack not empty"

    regioncorr-list-tests
    depth 0<> abort" regioncorr-list tests stack not empty"

    session-tests
    depth 0<> abort" Session tests stack not empty"

    test-end
;
