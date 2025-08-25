
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

\ TODO zero-token logic, like run a need.
: do-zero-token-command ( -- true )
    true
;

: do-one-token-commands ( c-addc c-cnt -- flag )
    2dup s" q" compare 0=
    if
        2drop
        false
    else
        2drop
        true
    then
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
        \ Default, clear stack.
        cr ." Token count does not correspond to any allowable ecommand" cr
        0 do 2drop loop
        true
    endcase
;

\ Get input of up to TOS characters from user, using PAD area.
\ Evaluate the input.
\ like: 80 s" Enter command: q(uit), ... > " get-user-input 
: get-user-input ( n c-addr cnt -- )
        cr
        \ Display the prompt.
        type                    \ n
        \ Get chars, leaves num chars on TOS.
        pad dup rot accept      \ pad-addr pad-addr n
                                \ pad-addr c-cnt
        cr
        parse-user-input         \ [ c-addr c-cnt ]* token-cnt
        eval-user-input          \ [ c-addr c-cnt ]* token-cnt
;

: input-test-parse-user-input
    \ Straight-forward test.
    s" ab cde fghi" parse-user-input
    3 <>
    abort" Three tokens not found"

    s" ab"   compare 0<> abort" ab not found"
    s" cde"  compare 0<> abort" cde not found"
    s" fghi" compare 0<> abort" fghi not found"

    depth 0<> abort" Test 1 stack not empty"
    
    \ Double up separators, and at start and end.
    s"  ab cde  fghi " parse-user-input
    3 <>
    abort" Three tokens not found"

    s" ab"   compare 0<> abort" ab not found"
    s" cde"  compare 0<> abort" cde not found"
    s" fghi" compare 0<> abort" fghi not found"

    depth 0<> abort" Test 2 stack not empty"
    
    \ Try no string.
    s" " parse-user-input
    0 <>
    abort" No tokens not found"

    depth 0<> abort" Test 3 stack not empty"
    
    \ Try only one separator.
    s"  " parse-user-input
    0 <>
    abort" No tokens not found"

    depth 0<> abort" Test 4 stack not empty"
    
    \ Try only two separators.
    s"   " parse-user-input
    0 <>
    abort" No tokens not found"

    depth 0<> abort" Test 5 stack not empty"

    cr ." input-test-parse-user-input: Ok" cr
;

