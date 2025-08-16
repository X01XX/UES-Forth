\ Tests for the rule struct functions.

: rule-test-restrict-initial-region
    7 6 region-new                  \ reg67

    4 5 rule-new                    \ reg67 rul5
    12 15 rule-new                  \ reg67 rul5 rulf
    2dup rule-union                 \ reg67 rul5 rul15 rulu
    0= abort" rule union failed?"

    swap    rule-deallocate         \ reg67 rul5 rulu
    swap    rule-deallocate         \ reg67 rulu

    2dup                            \ reg67 rulu reg67 rulu
    rule-restrict-initial-region    \ reg67 rulu rulr

    4 7 rule-new                    \ reg67 rulu rulr rul74
    2dup rule-eq
    0= abort" rules ne?"

    rule-deallocate
    rule-deallocate
    rule-deallocate
    region-deallocate

    cr ." rule-test-restrict-initial-region - Ok" cr
;

: rule-test-restrict-result-region
    4 0 region-new                  \ reg40

    4 5 rule-new                    \ reg40 rul5
    12 15 rule-new                  \ reg40 rul5 rulf
    2dup rule-union                 \ reg40 rul5 rul15 rulu
    0= abort" rule union failed?"

    swap    rule-deallocate         \ reg40 rul5 rulu
    swap    rule-deallocate         \ reg40 rulu

    2dup                            \ reg40 rulu reg40 rulu
    rule-restrict-result-region     \ reg40 rulu rulr

    dup rule-initial-region         \ reg40 rulu rulr regri
    
    5 7 region-new                  \ reg40 rulu rulr regri reg57
    2dup region-eq
    0= abort" rule initial region ne?"

    region-deallocate
    region-deallocate
    rule-deallocate
    rule-deallocate
    region-deallocate

    cr ." rule-test-restrict-result-region - Ok" cr
;

: rule-tests
    rule-test-restrict-initial-region
    rule-test-restrict-result-region
;

