\ Support the indirect use of functions, due to a function being
\ referenced before being defined.
\
\ Source code reordering can sometimes solve this, but it is not always easy, or aesthetic, and can change with new code.
\
\ So a place holder is defined here, like: ' xtabort value <function-name>-xt
\
\ In a file before a function is defined, use a line like: "<function-name>-xt execute" instead of "<function-name>"
\
\ After the function is defined, add a line like: ' <function-name> to <function-name>-xt
\

: xtabort
    ." indirect xt not initialized"
    abort
;

' xtabort value session-set-current-domain-xt

' xtabort value session-get-number-domains-xt

' xtabort value session-get-domain-list-xt


' xtabort value cur-domain-xt


' xtabort value domain-get-all-bits-mask-xt

' xtabort value domain-get-ms-bit-mask-xt

' xtabort value domain-get-inst-id-xt

' xtabort value domain-get-max-region-xt

' xtabort value domain-set-current-xt

' xtabort value domain-get-current-state-xt

' xtabort value domain-get-sample-xt


' xtabort value assert-tos-is-domain-xt

' xtabort value assert-nos-is-domain-xt

' xtabort value domain-get-num-bits-xt


' xtabort value cur-action-xt

' xtabort value action-get-inst-id-xt

' xtabort value assert-tos-is-action-xt

' xtabort value assert-nos-is-action-xt

' xtabort value action-make-need-xt


' xtabort value memory-use-xt

' xtabort value step-list-push-xt

' xtabort value step-new-xt

' xtabort value step-set-forward-xt

' xtabort value region-list-push-xt


' xtabort value .rlcrate-xt


