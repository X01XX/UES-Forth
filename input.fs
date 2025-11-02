
: do-need ( ned -- )
    \ Check arg.
    assert-tos-is-need

    dup need-get-action         \ ned act
    over need-get-domain        \ ned act dom

    \ Set cur domain.
    dup                         \ ned act dom dom
    current-session             \ ned act dom dom sess
    session-set-current-domain  \ ned act dom

    \ See if a plan is needed.
    dup domain-get-current-state    \ ned act dom d-sta
    #3 pick need-get-target         \ ned act dom d-sta n-sta
    =                               \ ned act dom flag
    if
        \ No plan needed, get sample.
        2dup                        \ ned act dom act dom
        domain-get-sample           \ ned act dom sample
        sample-deallocate               \ ned act dom
        domain-get-inst-id
        cr ." Dom: " dec.               \ ned act
        .action cr                      \ ned
        drop                            \
    else                                \ ned act dom
        #2 pick need-get-target         \ ned act dom t-sta
        over domain-get-current-state   \ ned act dom t-sta c-state
        sample-new                      \ ned act dom smpl
        2dup swap                       \ ned act dom smpl smpl dom
        domain-get-plan                 \ ned act dom smpl, plan true | false
        if
            dup plan-run                \ ned act dom smpl plan flag
            if
                cr ." plan succeeded" cr
                                        \ ned act dom smpl plan
                #3 pick                 \ ned act dom smpl plan act
                #3 pick                 \ ned act dom smpl plan act dom
                domain-get-sample       \ ned act dom smpl plan smpl
                sample-deallocate       \ ned act dom smpl plan
            else
                cr ." plan failed" cr
            then
                                        \ ned act dom smpl plan
            plan-deallocate
        else
            cr ." No plan found" cr
        then
                                        \ ned act dom smpl
        sample-deallocate               \ ned act dom
        2drop drop
    then
;

: do-to-command ( rlc0 -- )
    current-session session-get-current-states   \ rlc0 sta-corr

    \ Check if the current states are already at the goal.
    2dup swap                                   \ rlc0 sta-corr sta-corr rlc0
    region-list-corr-superset-states            \ rlc0 sta-corr bool
    if
        cr ." The current states are already at goal." cr
        list-deallocate
        drop
        exit
    then
                                                \ rlc0 sta-corr
    2dup swap region-list-corr-translate-states \ rlc0 sta-lst sta-lst2
                                                \ rlc0 sta-corr sta-lst2
    swap samplecorr-new                         \ rlc0 smplcor
    cr ." Desired sample corr: " dup .samplecorr cr
    samplecorr-deallocate

    drop
;

\ Zero-token logic, get/show/act-on needs.
: do-zero-token-command ( -- true )
    current-session             \ sess
    session-get-needs           \ ned-lst

    dup list-get-length         \ ned-lst len
    0=
    if
        \ ." No needs found" cr
        drop
        true
        exit
    then

    \ Look for a need that is satisfied by the current state.
                                \ ned-lst
    dup list-get-links          \ ned-lst link

    begin
        ?dup
    while
        dup link-get-data           \ ned-lst link nedx
        dup need-get-domain         \ ned-lst link nedx n-dom
        domain-get-current-state    \ ned-lst link nedx dom-sta
        over need-get-target        \ ned-lst link nedx dom-sta ned-sta
        = if                        \ ned-lst link nedx
            cr ." Need chosen: " space dup .need cr
            nip nip                 \ nedx
            do-need                 \
            true
            exit
        else                        \ ned-lst link nedx
            drop                    \ ned-lst link
        then

        link-get-next
    repeat                          \ ned-lst

    dup list-get-length             \ ned-lst len
    random                          \ ned-lst indx

    swap list-get-item              \ nedx
    cr ." Need chosen: " space dup .need cr
    do-need                         \

    true
;

: do-one-token-commands ( c-addr c-cnt -- flag )
    \ Quit.
    2dup s" q" str=
    if
        \ Clear token
        2drop
        \ Return continue loop flag.
        false
        exit
    then
    \ Print Session.
    2dup s" ps" str=
    if
        2drop
        current-session .session
        true
        exit
    then
    2dup s" mu" str=
    if
        2drop
        memory-use-xt execute
        true
        exit
    then

    2dup snumber?
    if
        nip nip
        \ cr dup ." You entered number " . cr

        \ Check lower bound.
        dup 0 <                             \ n flag
        if
            cr ." Number entered is LT zero" cr
            drop true exit
        then                                \ n

        \ Check higher bound.
        current-session                     \ n sess
        session-get-needs                   \ n ned-lst
        dup list-get-length                 \ n ned-lst ned-len
        #2 pick                             \ n ned-lst ned-len n
        swap                                \ n ned-lst n ned-len
        >=
        if                                  \ n ned-lst flag
            cr ." Number entered is GE need list length" cr
            2drop true exit
        then                                \ n ned-lst

        \ Get selected need.
        list-get-item                       \ ned
        cr ." You chose need: " dup .need cr
        do-need
        true
        exit
    then

    cr ." One-token command not recognized" cr
    \ Clear token.
    2drop
    \ Return continue loop flag.
    true
;

: do-two-token-commands ( c-addr c-cnt c-addr c-cnt -- flag )
    \ Print Dmain.
    2dup s" pd" str=
    if
        \ Drop command string.
        2drop                                       \ c-addr c-cnt
        \ Get domain ID.
        snumber?
        if
            current-session session-find-domain     \ dom t | f
            if
                \ Set current domain.
                dup current-session session-set-current-domain
                .domain
                true
                exit
            else
                cr ." pd command: domain number invalid" cr
                true
                exit
            then
        else
            cr ." pd command: domain number invalid" cr
            true
            exit
        then

        true
        exit
    then

    cr ." Two-token command not recognized" cr
    \ Clear tokens.
    2drop
    2drop
    \ Return continue loop flag.
    true
;

: do-three-token-commands ( c-addr c-cnt c-addr c-cnt c-addr c-cnt -- flag )
    \ Change Domain State: cds <domain id> state
    2dup s" cds" str=
    if
        2drop
        \ Get domain ID.
        snumber?
        if
            current-session session-find-domain     \ ... dom t | f
            if
                \ Set current domain.
                dup current-session session-set-current-domain

                -rot                                \ ... dom c-addr cnt
            else
                cr ." cds command: domain number invalid" cr
                2drop
                true
                exit
            then
        else
            cr ." cds command: domain number invalid" cr
            2drop
            true
            exit
        then

        \ Get state.
        snumber?                                    \ dom, sta t | f
        if
            dup is-not-value
            if
                cr ." cds command: state invalid" cr
                2drop
                true
                exit
            else
                swap                                \ sta dom
                domain-set-current-state            \
                true
                exit
            then
        else
            cr ." cds command: state invalid" cr
            drop
        then
        true
        exit
    then

    \ Print Square Detail: <domain id> <action id>
    2dup s" psd" str=
    if
        2drop
        \ Get domain ID.
        snumber?                                    \ ... dom-id t | f
        if
            \ cr ." domain " dup . cr
            current-session session-find-domain     \ ... dom t | f
            if
                \ Set current domain.
                dup current-session session-set-current-domain

                -rot                                \ ... dom c-addr cnt
            else
                cr ." psd command: domain number invalid" cr
                2drop
                true
                exit
            then
        else
            cr ." psd command: domain number invalid" cr
            2drop
            true
            exit
        then

        \ Get action.
        snumber?                                    \ ... dom, act-id t | f
        if
            \ cr ." action " dup . cr
            swap tuck domain-find-action            \ ... dom, act t | f
            if
                tuck swap                           \ ... act act dom
                domain-set-current-action           \ ... act
                action-get-squares                  \ ... sqr-lst
                cr .square-list cr
                true
                exit
            else
                cr ." psd command: action invalid" cr
                drop
            then
        else
            cr ." psd command: action invalid" cr
            drop
        then
        true
        exit
    then

    \ To State: tos <domain id> state
    2dup s" tos" str=
    if
        2drop
        \ Get domain ID.
        snumber?
        if
            current-session session-find-domain     \ ... dom t | f
            if
                \ Set current domain.
                dup current-session session-set-current-domain

                -rot                                \ ... dom c-addr cnt
            else
                cr ." tos command: domain number invalid" cr
                2drop
                true
                exit
            then
        else
            cr ." tos command: domain number invalid" cr
            2drop
            true
            exit
        then

        \ Get state.
        snumber?                                    \ dom, sta t | f
        if
            dup is-not-value
            if
                cr ." tos command: state invalid" cr
                2drop
                true
                exit
            else
                swap                                \ sta dom
                dup domain-get-current-state        \ sta dom cur-sta
                rot swap                            \ dom sta cur-sta
                2dup =
                if                                  \ dom sta cur-sta
                    cr ." Already at that state."
                    3drop
                    true
                    exit
                then

                \ Do domain-get-plan
                sample-new                          \ dom smpl
                tuck swap                           \ smpl smpl dom
                domain-get-plan                     \ smpl, plan t | f
                if                                  \ smpl plan
                    swap sample-deallocate          \ plan
                    dup                             \ plan plan
                    plan-run                        \ plan flag
                    swap plan-deallocate            \ flag
                    if
                        cr ." Plan succeeded" cr
                    else
                        cr ." Plan failed" cr
                    then
                else                                \ smpl
                    sample-deallocate               \
                    cr ." No plan found" cr
                then

                true
                exit
            then
        else
            cr ." tos command: state invalid" cr
            drop
        then
        true
        exit
    then

    \ Sample Current State: scs <domain-id> <action-id>
    2dup s" scs" str=
    if
        2drop
        \ Get domain ID.
        snumber?                                    \ ... dom-id t | f
        if
            \ cr ." domain " dup . cr
            current-session session-find-domain     \ ... dom t | f
            if
                \ Set current domain.
                dup current-session session-set-current-domain

                -rot                                \ ... dom c-addr cnt
            else
                cr ." scs command: domain number invalid" cr
                2drop
                true
                exit
            then
        else
            cr ." scs command: domain number invalid" cr
            2drop
            true
            exit
        then

        \ Get action.
        snumber?                                    \ dom, act-id t | f
        if
            \ cr ." action " dup . cr
            swap tuck domain-find-action            \ dom, act t | f
            if
                swap 2dup                           \ act dom act dom
                domain-set-current-action           \ act dom
                dup domain-get-current-state        \ act dom cur-sta
                rot                                 \ dom cur-sta act
                action-get-sample                   \ dom smpl
                dup sample-get-result               \ dom smpl smpl-r
                rot                                 \ smpl smpl-r dom
                domain-set-current-state            \ smpl
                sample-deallocate                   \
                true
                exit
            else
                drop
                cr ." scs command: action invalid" cr
            then
        else
            drop
        then
        true
        exit
    then

    \ Print Action.
    2dup s" pa" str=
    if
        \ Drop command string.
        2drop                                       \ c-addr c-cnt c-addr c-cnt
        \ Get domain ID.
        snumber?
        if
            current-session session-find-domain     \ c-addr c-cnt dom t | f
            if
                \ Set current domain.
                dup current-session session-set-current-domain
            else
                cr ." pa command: domain number invalid" cr
                2drop
                true
                exit
            then
        else
            cr ." pa command: domain number invalid" cr
            2drop
            true
            exit
        then
        -rot                                        \ dom c-addr c-cnt

        \ Get action.
        snumber?                                    \ dom, act-id t | f
        if                                          \ dom act-id
            over domain-find-action                 \ dom, act t | f
            if                                      \ dom act
                tuck swap                           \ act act dom
                domain-set-current-action           \ act
                .action
                true
                exit
            else
                cr ." pa command: action invalid" cr
                drop
                true
                exit
            then
        else
            cr ." pa command: action invalid" cr
            drop
            true
            exit
        then                    

        true
        exit
    then

    \ Change current rlc to another.
    2dup s" to" str=
    if
        \ Drop command string.
        2drop                                       \ c-addr c-cnt c-addr c-cnt

        \ Get goal rlc.
        2                                           \ add new length, 2.
        region-list-corr-from-parsed-string         \ rlc t | f

        if
            dup do-to-command
            region-list-deallocate
        else
            cr ." to command: Did not understand the given rlc string" cr
        then
        true
        exit
    then
    
    cr ." Three-token command not recognized" cr
    \ Clear tokens.
    2drop
    2drop
    2drop
    \ Return continue loop flag.
    true
;

: do-four-token-commands ( c-addr c-cnt c-addr c-cnt c-addr c-cnt c-addr c-cnt -- flag )
    \ Sample Arbitrary State: ss <domain id> <action id> state
    2dup s" sas" str=
    if
        2drop                                       \ sta-str act-id-str dom-id-str
        \ Get domain ID.
        snumber?                                    \ sta-str act-id-str dom-id t | f
        if
            \ cr ." domain " dup . cr
            current-session session-find-domain     \ sta-str act-id-str dom t | f
            if
                \ Set current domain.
                dup current-session session-set-current-domain

                -rot                                \ sta-str dom act-id-str
            else
                cr ." sas command: domain number invalid" cr
                2drop
                2drop
                true
                exit
            then
        else
            cr ." sas command: domain number invalid" cr
            2drop
            2drop
            true
            exit
        then

        \ Get action.
        snumber?                                    \ sta-str dom, act-id t | f
        if
            \ cr ." action " dup . cr
            swap tuck domain-find-action            \ sta-str dom, act t | f
            if
                swap 2dup                           \ sta-str act dom act dom
                domain-set-current-action           \ sta-str act dom
            else
                cr ." sas command: action invalid" cr
                3drop
                true
                exit
            then
        else
            cr ." sas command: action invalid" cr
            3drop
            true
            exit
        then                                        \ sta-str act dom

        \ Get state.
        2swap                                       \ act dom sta-str
        snumber?                                    \ act dom, sta t | f
        if                                          \ act dom sta
            dup is-not-value                        \ act dom sta flag
            if
                cr ." sas command: invalid state"
                3drop
                true
                exit
            then

            2dup swap                           \ act dom sta sta dom
            domain-set-current-state            \ act dom sta
            rot                                 \ dom sta act
            action-get-sample                   \ dom smpl
            dup sample-get-result               \ dom smpl smpl-r
            rot                                 \ smpl smpl-r dom
            domain-set-current-state            \ smpl
            sample-deallocate
            true
            exit
        else                                        \ act dom
            cr ." sas command: invalid state"
            2drop
            true
            exit
        then
    then

    cr ." Four-token command not recognized" cr
    \ Clear tokens.
    2drop
    2drop
    2drop
    2drop
    \ Return continue loop flag.
    true
;

\ Do commands from user input.
\ Return true if the read-eval loop should continue.
: eval-user-input ( [ c-addr c-cnt ]* token-cnt -- flag )
    \ cr ." eval-user-input: " .s cr
    \ Check for no tokens
    dup 0=
    if
        drop
        do-zero-token-command
        exit
    then

    \ Check the number of tokens.
    dup
    case
        1 of
            drop
            do-one-token-commands
        endof
        #2 of
            drop
            do-two-token-commands
        endof
        #3 of
            drop
            do-three-token-commands
        endof
        #4 of
            drop
            do-four-token-commands
        endof
        \ Default.
        cr ." Token count does not correspond to any allowable command" cr
        \ Clear tokens.
        0 do 2drop loop
        \ Return continue loop flag.
        true
    endcase
;

\ Get input of up to TOS characters from user, using the PAD area, up to a given number of characters.
\ Evaluate the input.
\ like: 80 s" Enter command: > " get-user-input
\
\ If this aborts, various things can be done:
\
\ Print all domains, and actions.
\   current-session  .session
\
\ Print Domain 1.
\    1  current-session  session-find-domain  drop  .domain
\
\ Print Domain 1, Act 4.
\    1  current-session  session-find-domain  drop  4  swap  domain-find-action  drop  .action
\
\ Print the squares of domain 1 action 4.
\    1  current-session  session-find-domain  drop  4  swap  domain-find-action  drop  action-get-squares  .square-list
: get-user-input ( n c-addr cnt -- )

        \ Display needs.
        current-session             \ n c-addr cnt sess
        dup session-set-all-needs   \ n c-addr cnt sess
        session-get-needs           \ n c-addr cnt ned-lst
        dup list-get-length         \ n c-addr cnt ned-lst len
        dup 0=
        if
            cr ." Needs: No needs found" cr
            2drop
        else
            drop
            cr ." Needs:" cr .need-list cr  \ n c-addr cnt
            cr ." Press Enter to randomly choose a need."
            cr ." <number> - Select a particular need."
        then

        cr ." q - to quit"
        cr
        cr ." ps - Print Session, all domains."
        cr ." pd <domain id> - Print Domain."
        cr ." pa <domain id> <action id> - Print Action."
        cr ." cds <domain ID> <state> - Change Domain current State, to an arbitrary value."
        cr ." tos <domain ID> <state> - TO domain State, from the current state, to an arbtrary value, by finding and executing a plan."
        cr ." psd <domain ID> <action ID> - Print Square Detail, for a given domain/action."
        cr ." scs <domain id> <action id> - Sample the Current State of a domain, with an action."
        cr ." sas <domain id> <action id> <state> - Sample an Arbitrary State. Change domain current state, then sample with an action."
        cr ." to - Change all domain states, like: to (0X00 000X1)"
        cr ." mu - Display Memory Use."
        cr
        cr ." <state> will usually be like: %0101, leading zeros can be ommitted."
        cr

        \ Display the prompt.
        cr
        type                    \ n
        \ Get chars, leaves num chars on TOS.
        pad dup rot accept      \ pad-addr pad-addr n
                                \ pad-addr c-cnt
        cr
        parse-string            \ [ c-addr c-cnt ]* token-cnt
        eval-user-input         \ [ c-addr c-cnt ]* token-cnt
;
