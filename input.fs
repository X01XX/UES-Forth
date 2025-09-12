
\ Load the stack with string info and token-count, like:
\ 0 or
\ c-addr c-cnt 1 or
\ c-addr c-cnt c-addr c-cnt 2 or
\ c-addr c-cnt c-addr c-cnt c-addr c-cnt 3 or ...
\
\ The stack will have the token info top-down as in the string left-to-right.
: parse-user-input ( c-start c-cnt -- [c-addr c-cnt ] token-cnt )
    \ Check for null input.
    dup 0= if           \ c-start 0
        nip
        exit
    then    

    \ Prep for loop.    \ c-start c-cnt

    \ Calc string end address.
    2dup + 1-           \ c-start c-cnt c-end 

    \ Init token counter.
    0 swap              \ c-start fl t-cnt c-end    \ c-cnt is a filler at this point, so -2rot works.

    \ Setup loop range
    3 pick              \ c-start fl t-cnt c-end c-start
    over                \ c-start fl t-cnt c-end c-start c-end

    \ Scan the string, end to start.
    \ Loop i value is the current possible start value.
    do                              \ c-start fl t-cnt c-end
        \ Get next char
        i c@                        \ c-start fl t-cnt c-end char

        \ Check for a separator.
        bl =                        \ c-start fl t-cnt c-end flag
        if
            \ Check length GT zero.
            dup i -                 \ c-start fl t-cnt c-end s-len
            dup                     \ c-start fl t-cnt c-end s-len s-len
            if                      \ c-start fl t-cnt c-end s-len

                \ Calc token start.
                i  1+               \ c-start fl t-cnt c-end s-len t-start

                \ Prep token definition.
                swap                \ c-start fl t-cnt c-end t-start t-len

                \ Store token def deeper into stack.
                -2rot               \ c-start fl t-cnt c-end 

                \ Update token count.
                swap 1+ swap        \ c-start fl t-cnt c-end
            else                    \ c-start fl t-cnt c-end s-len
                drop                \ c-start fl t-cnt c-end
            then

            \ Restart c-end
            drop i 1-               \ c-start fl t-cnt c-end
        then

        \ Count string index down.
        -1
    +loop
                                    \ c-start fl t-cnt c-end

    \ Check for the last token, if any.

    \ Check length GT zero.
    dup                             \ c-start fl t-cnt c-end c-end
    4 pick                          \ c-start fl t-cnt c-end c-end c-start
    - 1+                            \ c-start fl t-cnt c-end t-cnt
    dup                             \ c-start fl t-cnt c-end t-cnt t-cnt
    if                              \ c-start fl t-cnt c-end t-cnt
        \ Pepare string definition.
        4 pick                      \ c-start fl t-cnt c-end t-cnt t-start
        swap                        \ c-start fl t-cnt c-end t-start t-cnt

        \ Store token def deeper into stack.
        -2rot                       \ c-start fl t-cnt c-end

        \ Update token count.
        drop                        \ c-start fl t-cnt
        1+                          \ c-start fl t-cnt
    else                            \ c-start fl t-cnt c-end t-cnt
        2drop                       \ c-start fl t-cnt
    then
    \ Clear stack
    nip nip                         \ t-cnt
;

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
    3 pick need-get-target          \ ned act dom d-sta n-sta
    =                               \ ned act dom flag
    if
        \ No plan needed, get sample.
        2dup                        \ ned act dom act dom
        domain-get-sample           \ ned act dom sample
        sample-deallocate           \ ned act dom
        domain-get-inst-id
        cr ." Dom: " .              \ ned act
        .action cr                  \ ned
        drop                        \
    else                            \ ned act dom
        2 pick need-get-target      \ ned act dom t-sta
        over domain-get-current-state   \ ned act dom t-sta c-state
        sample-new                  \ ned act dom smpl
        2dup swap                   \ ned act dom smpl smpl dom
        domain-get-plan    \ ned act dom smpl, plan true | false
        if
            dup plan-run                \ ned act dom smpl plan flag
            if
                cr ." plan succeeded" cr
                                        \ ned act dom smpl plan
                3 pick                  \ ned act dom smpl plan act
                3 pick                  \ ned act dom smpl plan act dom
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

\ Zero-token logic, get/show/act-on needs.
: do-zero-token-command ( -- true )
    current-session             \ sess
    dup session-get-needs       \ sess ned-lst

    dup list-get-length         \ sess ned-lst len
    ?dup 0=
    if
        \ ." No needs found" cr
        2drop
    else
        random                      \ sess ned-lst indx

        dup cr ." Need chosen: " . space
        swap list-get-item          \ sess ned
        dup .need cr                \ sess ned

        nip                         \ ned
        do-need                     \
    then
    true
;

: do-one-token-commands ( c-addc c-cnt -- flag )
    2dup s" q" str=
    if
        \ Clear token
        2drop
        \ Return continue loop flag.
        false
        exit
    then
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
        2 pick                              \ n ned-lst ned-len n
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
        \ Default.
        cr ." Token count does not correspond to any allowable command" cr
        \ Clear tokens.
        0 do 2drop loop
        \ Return continue loop flag.
        true
    endcase
;

\ Get input of up to TOS characters from user, using PAD area.
\ Evaluate the input.
\ like: 80 s" Enter command: q(uit), ... > " get-user-input 
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
        then

        cr ." q - to quit"
        cr ." Press Enter to randomly choose a need, if any." 
        cr ." ps - to Print Session, all domains, actions."
        cr ." mu - Display Memory Use."
        cr ." <number> - Select a particular need."
        cr

        \ Display the prompt.
        cr
        type                    \ n
        \ Get chars, leaves num chars on TOS.
        pad dup rot accept      \ pad-addr pad-addr n
                                \ pad-addr c-cnt
        cr
        parse-user-input         \ [ c-addr c-cnt ]* token-cnt
        eval-user-input          \ [ c-addr c-cnt ]* token-cnt
;
