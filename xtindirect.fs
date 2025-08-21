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

' xtabort value cur-domain-all-bits-xt

' xtabort value cur-domain-ms-bit-xt

' xtabort value cur-domain-max-region-xt

' xtabort value cur-domain-inst-id-xt

' xtabort value cur-domain-current-state-xt

' xtabort value domain-get-current-action-xt


' xtabort value cur-action-inst-id-xt

' xtabort value region-new-xt

' xtabort value region-subtract-state-xt

' xtabort value region-deallocate-xt


' xtabort value region-list-set-union-xt

' xtabort value region-list-deallocate-xt

' xtabort value region-list-push-xt

' xtabort value region-list-subtract-region-xt

' xtabort value session-get-current-domain-xt







